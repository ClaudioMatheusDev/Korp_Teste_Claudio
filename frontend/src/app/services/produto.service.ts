import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Produto, ProdutoCriar } from '../models/produto.model';

@Injectable({ providedIn: 'root' })
export class ProdutoService {
  private readonly baseUrl = `${environment.estoqueApiUrl}/produto`;

  constructor(private readonly http: HttpClient) {}

  listar(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.baseUrl);
  }

  criar(produto: ProdutoCriar): Observable<{ idProduto: number }> {
    return this.http.post<{ idProduto: number }>(this.baseUrl, produto);
  }
}
