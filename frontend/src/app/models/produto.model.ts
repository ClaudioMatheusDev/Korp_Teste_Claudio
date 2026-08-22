export interface Produto {
  idProduto: number;
  codigo: number;
  descricao: string;
  valorProduto: number;
  quantidadeEstoque: number;
  dataCriacao: string;
  dataAtualizacao: string | null;
}

export interface ProdutoCriar {
  codigo: number;
  descricao: string;
  valorProduto: number;
  quantidadeEstoque: number;
}
