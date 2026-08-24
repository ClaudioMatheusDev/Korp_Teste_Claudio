namespace Estoque.Application.Exceptions
{
    /// <summary>
    /// Indica que a operação falhou porque outro processo alterou os
    /// mesmos dados entre a leitura e a gravação (conflito de concorrência
    /// otimista). Quem chamar deve recarregar o estado atual e reavaliar
    /// a operação antes de tentar de novo.
    /// </summary>
    public class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException(string message) : base(message)
        {
        }

        public ConcurrencyConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
