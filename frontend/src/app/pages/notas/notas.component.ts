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
        this.mostrarErro(err);
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
        this.snackBar.open('Nota fiscal criada com sucesso.', 'OK', { duration: 3000 });
        this.carregarNotas();
      },
      error: (err: HttpErrorResponse) => {
        this.salvando = false;
        this.mostrarErro(err);
      },
    });
  }

  imprimir(nota: NotaFiscal): void {
    this.imprimindoId = nota.idNotaFiscal;

    this.notaFiscalService.imprimir(nota.idNotaFiscal).subscribe({
      next: () => {
        this.imprimindoId = null;
        this.snackBar.open('Nota fiscal impressa e fechada com sucesso.', 'OK', { duration: 3000 });
        this.carregarNotas();
      },
      error: (err: HttpErrorResponse) => {
        this.imprimindoId = null;
        this.mostrarErro(err);
      },
    });
  }

  private mostrarErro(err: HttpErrorResponse): void {
    const mensagem =
      err.status === 0
        ? 'Não foi possível se conectar ao servidor. Verifique sua conexão e tente novamente.'
        : err.error?.detail ?? err.error?.title ?? err.message ?? 'Erro desconhecido.';
    this.snackBar.open(mensagem, 'Fechar', { duration: 8000 });
  }
}
