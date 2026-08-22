import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { NotaFiscal, NotaFiscalCriar, NotaFiscalDetalhes } from '../models/nota-fiscal.model';

@Injectable({ providedIn: 'root' })
export class NotaFiscalService {
  private readonly baseUrl = `${environment.faturamentoApiUrl}/notas`;

  constructor(private readonly http: HttpClient) {}

  listar(): Observable<NotaFiscal[]> {
    return this.http.get<NotaFiscal[]>(this.baseUrl);
  }

  buscarPorId(idNotaFiscal: number): Observable<NotaFiscalDetalhes> {
    return this.http.get<NotaFiscalDetalhes>(`${this.baseUrl}/${idNotaFiscal}`);
  }

  criar(notaFiscal: NotaFiscalCriar): Observable<{ idNotaFiscal: number }> {
    return this.http.post<{ idNotaFiscal: number }>(this.baseUrl, notaFiscal);
  }

  imprimir(idNotaFiscal: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/${idNotaFiscal}/imprimir`, {});
  }
}
