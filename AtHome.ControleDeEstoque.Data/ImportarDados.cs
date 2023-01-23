using AtHome.ControleDeEstoque.Domain;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtHome.ControleDeEstoque.Data
{
    public class ImportarDados
    {
        public void ExecutarImportacaoDados()
        {
            OleDbConnection _olecon;
            OleDbCommand _oleCmd;
            string _arquivo = @"C:\Temp\ImportarDadosControleEstoque\data controledeestoque full recuperado.xls";
            string _stringConexao = "";

            _stringConexao = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;ReadOnly=False';", _arquivo);

            _olecon = new OleDbConnection(_stringConexao);
            _olecon.Open();

            _oleCmd = new OleDbCommand();
            _oleCmd.Connection = _olecon;

            var _grupoDAO = new GrupoDAO();
            var _itemDAO = new ItemDAO();
            var _pedidoDAO = new PedidoDAO();

            try
            {
                var itens_delete = _itemDAO.GetAll();
                foreach (var item in itens_delete)
                {
                    _itemDAO.Delete(item);
                }

                var grupos_delete = _grupoDAO.GetAll();
                foreach (var grupo in grupos_delete)
                {
                    _grupoDAO.Delete(grupo);
                }

                _oleCmd.CommandText = @"SELECT item_desc, grupo_id, item_qtd_atual, item_estoque_minimo, item_tamanho, item_preco_custo, item_porcetagem_lucro, item_preco_venda
                                          FROM [tab_item$]";
                OleDbDataReader reader_item = _oleCmd.ExecuteReader();

                var itens = new List<Item>();

                while (reader_item.Read())
                {
                    var item = new Item();
                    item.Descricao = reader_item.GetString(0);
                    item.GrupoId = long.Parse(reader_item.GetValue(1).ToString());
                    item.Quantidade = long.Parse(reader_item.GetValue(2).ToString());
                    item.EstoqueMinimo = long.Parse(reader_item.GetValue(3).ToString());
                    item.Tamanho = reader_item.GetString(4);
                    item.PrecoCusto = double.Parse(reader_item.GetValue(5).ToString());
                    item.PorcentagemLucro = double.Parse(reader_item.GetValue(6).ToString());
                    item.PrecoVenda = double.Parse(reader_item.GetValue(7).ToString());

                    itens.Add(item);
                }
                reader_item.Close();

                _oleCmd.CommandText = "SELECT grupo_id, grupo_desc FROM [tab_grupo$]";
                OleDbDataReader reader_grupo = _oleCmd.ExecuteReader();
                var grupos_existentes = _grupoDAO.GetAll();

                while (reader_grupo.Read())
                {
                    var grupo = new Grupo();
                    var id_antigo = long.Parse(reader_grupo.GetValue(0).ToString()) * 1000000;
                    grupo.Descricao = reader_grupo.GetString(1);
                    
                    if (!grupos_existentes.Any(x => x.Descricao == grupo.Descricao))
                        _grupoDAO.Save(grupo);

                    foreach (var item in itens.Where(x => x.GrupoId * 1000000 == id_antigo))
                    {
                        item.GrupoId = grupo.Id;
                    }
                }
                reader_grupo.Close();

                var itens_existentes = _itemDAO.GetAll();
                foreach (var item in itens)
                {
                    if (!itens_existentes.Any(x => x.Descricao == item.Descricao))
                        _itemDAO.Save(item);
                }
                
                _oleCmd.CommandText = @"SELECT pedido_numero, pedido_data_hora, pedido_finalizado, pedido_tipo_operacao, pedido_observacao, pedido_valor
                                          FROM [tab_pedido$]";
                OleDbDataReader reader_pedido = _oleCmd.ExecuteReader();

                _oleCmd.CommandText = @"SELECT pedido_id, item_id, item_desc, pedido_item_tamanho, grupo_desc, pedido_quantidade, pedido_valor_unitario, pedido_valor_total
                                          FROM [tab_pedido_item$]";
                OleDbDataReader reader_pedido_item = _oleCmd.ExecuteReader();

                var itens_pedidos = new List<ItemDoPedido>();

                while (reader_pedido_item.Read())
                {
                    var item_pedido = new ItemDoPedido();
                    item_pedido.IdPedido = long.Parse(reader_pedido_item.GetValue(0).ToString());
                    item_pedido.IdItem = long.Parse(reader_pedido_item.GetValue(1).ToString());
                    item_pedido.Descricao = reader_pedido_item.GetString(2);
                    item_pedido.Tamanho = reader_pedido_item.GetString(3);
                    item_pedido.DescricaoGrupo = reader_pedido_item.GetString(4);
                    item_pedido.QuantidadePedido = long.Parse(reader_pedido_item.GetValue(5).ToString());
                    item_pedido.PrecoVenda = double.Parse(reader_pedido_item.GetValue(6).ToString());
                    item_pedido.ValorTotal = double.Parse(reader_pedido_item.GetValue(7).ToString());

                    itens_pedidos.Add(item_pedido);
                }
                reader_pedido_item.Close();

                while (reader_pedido.Read())
                {
                    var pedido = new Pedido();
                    pedido.Id = long.Parse(reader_pedido.GetValue(0).ToString());
                    pedido.Numero = long.Parse(reader_pedido.GetValue(0).ToString());
                    pedido.DataHora = DateTime.Parse(reader_pedido.GetDateTime(1).ToString());
                    pedido.PedidoFinalizado = bool.Parse(reader_pedido.GetValue(2).ToString());
                    pedido.TipoOperacao = (Enumeradores.TipoOperacao)int.Parse(reader_pedido.GetValue(3).ToString());
                    pedido.Observacao = reader_pedido.GetString(4);
                    pedido.Valor = double.Parse(reader_pedido.GetValue(5).ToString());
                    pedido.ItensDoPedido = itens_pedidos.Where(x => x.IdPedido == pedido.Id).ToList();

                    _pedidoDAO.Save(pedido);

                }
                reader_pedido.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
