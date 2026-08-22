export enum StatusNotaFiscal {
  Aberta = 1,
  Fechada = 2,
}

export interface ItemNotaFiscal {
  idItemNotaFiscal: number;
  idProduto: number;
  quantidade: number;
}

export interface ItemNotaFiscalCriar {
  idProduto: number;
  quantidade: number;
}

export interface NotaFiscal {
  idNotaFiscal: number;
  numero: number;
  status: StatusNotaFiscal;
  dataCriacao: string;
  dataFechamento: string | null;
}

export interface NotaFiscalDetalhes extends NotaFiscal {
  itens: ItemNotaFiscal[];
}

export interface NotaFiscalCriar {
  itens: ItemNotaFiscalCriar[];
}
