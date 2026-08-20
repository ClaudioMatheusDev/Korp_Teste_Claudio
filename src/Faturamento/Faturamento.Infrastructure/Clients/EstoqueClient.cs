using System.Net;
using System.Net.Http.Json;
using Faturamento.Application.Dtos;
using Faturamento.Application.Exceptions;
using Faturamento.Application.Interfaces;

namespace Faturamento.Infrastructure.Clients
{
    public class EstoqueClient : IEstoqueClient
    {
        private readonly HttpClient _httpClient;

        public EstoqueClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task BaixarEstoqueLoteAsync(BaixaEstoqueLoteDto dto)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync("api/estoque/baixar-lote", dto);
            }
            catch (HttpRequestException ex)
            {
                throw new EstoqueIndisponivelException(
                    "Não foi possível conectar ao serviço de Estoque.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new EstoqueIndisponivelException(
                    "O serviço de Estoque demorou demais para responder.", ex);
            }

            if (response.IsSuccessStatusCode)
                return;

            var mensagem = await ExtrairMensagemDeErroAsync(response);

            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.Conflict)
            {
                throw new BusinessRuleException(
                    $"O Estoque rejeitou a baixa: {mensagem}");
            }

            throw new EstoqueIndisponivelException(
                $"O serviço de Estoque retornou um erro inesperado ({(int)response.StatusCode}): {mensagem}");
        }

        private static async Task<string> ExtrairMensagemDeErroAsync(HttpResponseMessage response)
        {
            try
            {
                var problema = await response.Content.ReadFromJsonAsync<ProblemaDto>();
                return problema?.Detail ?? problema?.Title ?? problema?.Message ?? response.ReasonPhrase ?? "erro desconhecido";
            }
            catch
            {
                return response.ReasonPhrase ?? "erro desconhecido";
            }
        }

        private class ProblemaDto
        {
            public string? Title { get; set; }
            public string? Detail { get; set; }
            public string? Message { get; set; }
        }
    }
}
