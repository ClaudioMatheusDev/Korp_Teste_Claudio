import { MatSnackBar } from '@angular/material/snack-bar';

export function abrirSucesso(snackBar: MatSnackBar, mensagem: string): void {
  snackBar.open(mensagem, 'OK', { duration: 3000, panelClass: 'snackbar-sucesso' });
}

export function abrirErro(snackBar: MatSnackBar, mensagem: string): void {
  snackBar.open(mensagem, 'Fechar', { duration: 8000, panelClass: 'snackbar-erro' });
}
