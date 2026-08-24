using Faturamento.Application.Dtos;
using Faturamento.Application.Exceptions;
using Faturamento.Application.Interfaces;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Faturamento.Application.Services
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly INotaFiscalRepository _notaFiscalRepository;
        private readonly IEstoqueClient _estoqueClient;
        private readonly ILogger<NotaFiscalService> _logger;

        public NotaFiscalService(
            INotaFiscalRepository notaFiscalRepository,
            IEstoqueClient estoqueClient,
            ILogger<NotaFiscalService> logger)
        {
            _notaFiscalRepository = notaFiscalRepository;
            _estoqueClient = estoqueClient;
            _logger = logger;
        }

        public async Task<int> CriarNotaFiscalAsync(NotaFiscalCriarDto dto)
        {
            if (dto.Itens == null || dto.Itens.Count == 0)
            {
                throw new BusinessRuleException("A nota fiscal deve possuir pelo menos um item.");
            }

            if (dto.Itens.Any(i => i.Quantidade <= 0))
            {
                throw new BusinessRuleException("Todos os itens devem possuir quantidade maior que zero.");
            }

            var proximoNumero =
                await _notaFiscalRepository.BuscarProximoNumeroAsync();

            var notaFiscal = new NotaFiscal
            {
                Numero = proximoNumero,
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = DateTime.Now,

                Itens = dto.Itens
                    .Select(i => new ItemNotaFiscal
                    {
                        IDProduto = i.IDProduto,
                        Quantidade = i.Quantidade
                    })
                    .ToList()
            };

            await _notaFiscalRepository.AdicionarNotaAsync(notaFiscal);
            await _notaFiscalRepository.SalvarAlteracoesAsync();

            return notaFiscal.IDNotaFiscal;
        }


        public async Task<NotaFiscalDetalhesDto> BuscarNotaFiscalAsync(int IDNotaFiscal)
        {
            var notaFiscal =
                await _notaFiscalRepository
                    .BuscarNotaFiscalPorIDAsync(IDNotaFiscal);

            if (notaFiscal == null)
            {
                throw new NotFoundException("Nota fiscal não encontrada.");
            }

            return new NotaFiscalDetalhesDto
            {
                IDNotaFiscal = notaFiscal.IDNotaFiscal,
                Numero = notaFiscal.Numero,
                Status = notaFiscal.Status,
                DataCriacao = notaFiscal.DataCriacao,
                DataFechamento = notaFiscal.DataFechamento,

                Itens = notaFiscal.Itens
                    .Select(i => new ItemNotaFiscalResponseDto
                    {
                        IDItemNotaFiscal = i.IDItemNotaFiscal,
                        IDProduto = i.IDProduto,
                        Quantidade = i.Quantidade
                    })
                    .ToList()
            };
        }

        public async Task<List<NotaFiscalResponseDto>> ListarNotasFiscaisAsync()
        {
            var notaFiscal = await _notaFiscalRepository.ListarTodasNotasFiscais();


            return notaFiscal.Select(p => new NotaFiscalResponseDto
            {
                IDNotaFiscal = p.IDNotaFiscal,
                Numero = p.Numero,
                Status = p.Status,
                DataCriacao = p.DataCriacao,
                DataFechamento = p.DataFechamento
            }).ToList();
        }

        public async Task<bool> ImprimirNotaFiscal(int IDNotaFiscal)
        {
            var notaFiscal = await _notaFiscalRepository.BuscarNotaFiscalPorIDAsync(IDNotaFiscal);

            if (notaFiscal == null)
            {
                throw new NotFoundException("Nota fiscal não encontrada.");
            }

            if (notaFiscal.Status == StatusNotaFiscal.Fechada)
            {
                return false;
            }

            var baixaEstoque = new BaixaEstoqueLoteDto
            {
                IDNotaFiscal = notaFiscal.IDNotaFiscal,

                Itens = notaFiscal.Itens
            .Select(item => new ItemBaixaEstoqueDto
            {
                IDProduto = item.IDProduto,
                Quantidade = item.Quantidade
            })
            .ToList()
            };

            await _estoqueClient.BaixarEstoqueLoteAsync(baixaEstoque);

            notaFiscal.Status = StatusNotaFiscal.Fechada;
            notaFiscal.DataFechamento = DateTime.Now;

            try
            {
                await _notaFiscalRepository.SalvarAlteracoesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,
                    "Baixa de estoque da Nota Fiscal {IDNotaFiscal} foi confirmada no Estoque, " +
                    "mas falhou ao salvar o fechamento da nota no Faturamento. Requer reprocessamento.",
                    notaFiscal.IDNotaFiscal);

                throw;
            }

            return true;
        }
    }
}
