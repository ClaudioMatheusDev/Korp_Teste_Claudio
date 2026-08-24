import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotaFiscalService } from '../../services/nota-fiscal.service';
import { NotaFiscal, StatusNotaFiscal } from '../../models/nota-fiscal.model';
import { extrairMensagemErro } from '../../shared/http-error.util';
import { abrirErro, abrirSucesso } from '../../shared/feedback.util';

@Component({
  selector: 'app-notas',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './notas.component.html',
  styleUrl: './notas.component.scss',
})
export class NotasComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  readonly colunas = ['idNotaFiscal', 'numero', 'status', 'dataCriacao', 'dataFechamento', 'acoes'];
  readonly StatusNotaFiscal = StatusNotaFiscal;

  notas: NotaFiscal[] = [];
  carregando = false;
  salvando = false;

  /** IDNotaFiscal da nota sendo impressa agora, para mostrar o indicador só nela. */
  imprimindoId: number | null = null;

  /**
   * Erro da última tentativa de impressão, mantido visível na tela (não só
   * no snackbar) até o usuário tentar de novo — falha em imprimir é o
   * cenário central do requisito de tratamento de falhas entre
   * microsserviços, então precisa ficar visível o tempo que for preciso.
   */
  erroImpressao: { numeroNota: number; mensagem: string } | null = null;

  form = this.fb.group({
    itens: this.fb.array([this.criarItemForm()]),
  });

  constructor(private readonly notaFiscalService: NotaFiscalService) {}

  ngOnInit(): void {
    this.carregarNotas();
  }

  get itens(): FormArray {
    return this.form.get('itens') as FormArray;
  }

  criarItemForm() {
    return this.fb.group({
      idProduto: [null as number | null, [Validators.required, Validators.min(1)]],
      quantidade: [null as number | null, [Validators.required, Validators.min(1)]],
    });
  }

  adicionarItem(): void {
    this.itens.push(this.criarItemForm());
  }

  removerItem(index: number): void {
    if (this.itens.length > 1) {
      this.itens.removeAt(index);
    }
  }

  carregarNotas(): void {
    this.carregando = true;

    this.notaFiscalService.listar().subscribe({
      next: (notas) => {
        this.notas = notas;
        this.carregando = false;
      },
      error: (err: HttpErrorResponse) => {
        this.carregando = false;
        if (err.status === 404) {
          this.notas = [];
          return;
        }
        abrirErro(this.snackBar, extrairMensagemErro(err));
      },
    });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando = true;

    this.notaFiscalService.criar(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.salvando = false;
        this.itens.clear();
        this.itens.push(this.criarItemForm());
        abrirSucesso(this.snackBar, 'Nota fiscal criada com sucesso.');
        this.carregarNotas();
      },
      error: (err: HttpErrorResponse) => {
        this.salvando = false;
        abrirErro(this.snackBar, extrairMensagemErro(err));
      },
    });
  }

  imprimir(nota: NotaFiscal): void {
    this.imprimindoId = nota.idNotaFiscal;
    this.erroImpressao = null;

    this.notaFiscalService.imprimir(nota.idNotaFiscal).subscribe({
      next: (resposta) => {
        this.imprimindoId = null;
        abrirSucesso(this.snackBar, resposta.message);
        this.carregarNotas();
      },
      error: (err: HttpErrorResponse) => {
        this.imprimindoId = null;
        const mensagem = extrairMensagemErro(err);
        this.erroImpressao = { numeroNota: nota.numero, mensagem };
        abrirErro(this.snackBar, mensagem);
      },
    });
  }

  fecharAlertaImpressao(): void {
    this.erroImpressao = null;
  }
}
