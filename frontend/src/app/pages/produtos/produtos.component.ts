import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ProdutoService } from '../../services/produto.service';
import { Produto } from '../../models/produto.model';

@Component({
  selector: 'app-produtos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './produtos.component.html',
  styleUrl: './produtos.component.scss',
})
export class ProdutosComponent implements OnInit {
  produtos: Produto[] = [];
  carregando = false;
  salvando = false;
  erro: string | null = null;

  private readonly fb = inject(FormBuilder);

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
    this.erro = null;

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
          this.carregarProdutos();
        },
        error: (err: HttpErrorResponse) => {
          this.salvando = false;
          this.erro = this.extrairMensagemErro(err);
        },
      });
  }

  private extrairMensagemErro(err: HttpErrorResponse): string {
    return err.error?.detail ?? err.error?.title ?? err.message ?? 'Erro desconhecido.';
  }
}
