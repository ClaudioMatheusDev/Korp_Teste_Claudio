using System.Net.Http.Json;
using Faturamento.Application.Dtos;
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

        public async Task BaixarEstoqueLoteAsync(
            BaixaEstoqueLoteDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/estoque/baixar-lote",dto);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"Erro ao realizar baixa no estoque: {erro}");
            }
        }
    }
}