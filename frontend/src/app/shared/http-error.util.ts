import { HttpErrorResponse } from '@angular/common/http';

export function extrairMensagemErro(err: HttpErrorResponse): string {
  if (err.status === 0) {
    return 'Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente.';
  }

  if (err.status === 503) {
    return (
      err.error?.detail ??
      'Um dos serviços do sistema está indisponível no momento. Nada foi alterado — tente novamente em alguns instantes.'
    );
  }

  return err.error?.detail ?? err.error?.title ?? err.message ?? 'Erro desconhecido.';
}
