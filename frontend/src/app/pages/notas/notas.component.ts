import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { NotaFiscalService } from '../../services/nota-fiscal.service';
import { NotaFiscal, StatusNotaFiscal } from '../../models/nota-fiscal.model';

@Component({
  selector: 'app-notas',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './notas.component.html',
  styleUrl: './notas.component.scss',
})
export class NotasComponent implements OnInit {
  notas: NotaFiscal[] = [];
  carregando = false;
  salvando = false;
  erro: string | null = null;


  imprimindoId: number | null = null;

  readonly StatusNotaFiscal = StatusNotaFiscal;

  private readonly fb = inject(FormBuilder);

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
    this.erro = null;

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
        this.erro = this.extrairMensagemErro(err);
      },
    });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando = true;
    this.erro = null;

    this.notaFiscalService.criar(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.salvando = false;
        this.form.reset();
        this.itens.clear();
        this.itens.push(this.criarItemForm());
        this.carregarNotas();
      },
      error: (err: HttpErrorResponse) => {
        this.salvando = false;
        this.erro = this.extrairMensagemErro(err);
      },
    });
  }

  imprimir(nota: NotaFiscal): void {
    this.imprimindoId = nota.idNotaFiscal;
    this.erro = null;

    this.notaFiscalService.imprimir(nota.idNotaFiscal).subscribe({
      next: () => {
        this.imprimindoId = null;
        this.carregarNotas();
      },
      error: (err: HttpErrorResponse) => {
        this.imprimindoId = null;
        this.erro = this.extrairMensagemErro(err);
      },
    });
  }

  private extrairMensagemErro(err: HttpErrorResponse): string {
    return err.error?.detail ?? err.error?.title ?? err.message ?? 'Erro desconhecido.';
  }
}
