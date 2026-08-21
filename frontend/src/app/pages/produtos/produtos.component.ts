import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProdutoService } from '../../services/produto.service';
import { Produto } from '../../models/produto.model';

@Component({
  selector: 'app-produtos',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './produtos.component.html',
  styleUrl: './produtos.component.scss',
})
export class ProdutosComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  readonly colunas = ['idProduto', 'codigo', 'descricao', 'valorProduto', 'quantidadeEstoque'];

  produtos: Produto[] = [];
  carregando = false;
  salvando = false;

  form = this.fb.group({
    codigo: [null as number | null, [Validators.required, Validators.min(1)]],
    descricao: ['', [Validators.required, Validators.maxLength(200)]],
    valorProduto: [null as number | null, [Validators.required, Validators.min(0.01)]],
    quantidadeEstoque: [null as number | null, [Validators.required, Validators.min(0)]],
  });

  constructor(private readonly produtoService: ProdutoService) {}

  ngOnInit(): void {
    this.carregarProdutos();
  }

  carregarProdutos(): void {
    this.carregando = true;

    this.produtoService.listar().subscribe({
      next: (produtos) => {
        this.produtos = produtos;
        this.carregando = false;
      },
      error: (err: HttpErrorResponse) => {
        this.carregando = false;
        if (err.status === 404) {
          this.produtos = [];
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

    const valor = this.form.getRawValue();

    this.produtoService
      .criar({
        codigo: valor.codigo!,
        descricao: valor.descricao!,
        valorProduto: valor.valorProduto!,
        quantidadeEstoque: valor.quantidadeEstoque!,
      })
      .subscribe({
        next: () => {
          this.salvando = false;
          this.form.reset();
          this.snackBar.open('Produto cadastrado com sucesso.', 'OK', { duration: 3000 });
          this.carregarProdutos();
        },
        error: (err: HttpErrorResponse) => {
          this.salvando = false;
          this.mostrarErro(err);
        },
      });
  }

  private mostrarErro(err: HttpErrorResponse): void {
    const mensagem = err.error?.detail ?? err.error?.title ?? err.message ?? 'Erro desconhecido.';
    this.snackBar.open(mensagem, 'Fechar', { duration: 6000 });
  }
}
