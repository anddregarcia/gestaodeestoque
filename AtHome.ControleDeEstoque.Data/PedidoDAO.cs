using AtHome.ControleDeEstoque.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Text;

namespace AtHome.ControleDeEstoque.Data
{
    public class PedidoDAO
    {
        AppDbConnection _appConn;

        public long Save(Pedido data)
        {
            //StringBuilder sql = new StringBuilder();
            var sql = "";

            try
            {
                using (_appConn = new AppDbConnection())
                {
                    sql = @" insert  into  tab_pedido 
                            (pedido_numero, pedido_data_hora, pedido_finalizado, pedido_tipo_operacao, pedido_observacao, pedido_valor) 
                             values                            
                            (@pedido_numero, @pedido_data_hora, @pedido_finalizado, @pedido_tipo_operacao, @pedido_observacao, @pedido_valor) ";

                    System.Globalization.DateTimeFormatInfo dateInfoBr = new System.Globalization.DateTimeFormatInfo();
                    DateTime dataHoraPedido = Convert.ToDateTime(DateTime.Now, dateInfoBr);

                    Dictionary<string, dynamic> parametros = new Dictionary<string, dynamic>();

                    parametros.Add("@pedido_numero"         , data.Numero);
                    parametros.Add("@pedido_data_hora"      , dataHoraPedido);
                    parametros.Add("@pedido_finalizado"     , 0);
                    parametros.Add("@pedido_tipo_operacao"  , (int)data.TipoOperacao);
                    parametros.Add("@pedido_observacao"     , data.Observacao);
                    parametros.Add("@pedido_valor"          , data.Valor);
                    
                    var regs = _appConn.ExecuteNonQuery(sql, parametros);

                    sql = " select max(pedido_id) as pedido_id from tab_pedido ";
                    data.Id = _appConn.ExecuteScalar(sql.ToString());
                    
                    foreach (var item in data.ItensDoPedido)
                    {
                        sql = string.Format(
                            @"  insert  into  tab_pedido_item
                                    (pedido_id, item_id, item_desc, pedido_item_tamanho, grupo_desc, pedido_quantidade, pedido_valor_unitario, pedido_valor_total)
                                values
                                    ({0}, {1}, '{2}', '{3}', '{4}', {5}, {6}, {7})" , data.Id
                                                                                    , item.IdItem
                                                                                    , item.Descricao
                                                                                    , item.Tamanho
                                                                                    , item.DescricaoGrupo
                                                                                    , item.QuantidadePedido
                                                                                    , item.PrecoVenda.ToString().Replace(",", ".")
                                                                                    , item.ValorTotal.ToString().Replace(",", "."));

                        _appConn.ExecuteScalar(sql.ToString());

                    }
                }
                return data.Id;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public bool FinalizarPedido(long idPedido)
        {
            long regs = 0;
            StringBuilder sql = new StringBuilder();

            try
            {
                using (_appConn = new AppDbConnection())
                {
                    sql.Append(" update  tab_pedido ");
                    sql.Append(String.Format("     set  pedido_finalizado  =  1"));
                    sql.Append(String.Format("   where  pedido_id = {0}", idPedido));

                    regs = _appConn.ExecuteNonQuery(sql.ToString());
                }
            }
            catch (Exception)
            { throw; }

            return (regs > 0);
        }

        public List<Pedido> GetAllPedido()
        {
            StringBuilder sql = new StringBuilder();
            SqlCeDataReader sdr;
            var result = new List<Pedido>();

            try
            {
                using (_appConn = new AppDbConnection())
                {
                    sql.Append(" select a.pedido_id, ");
                    sql.Append(String.Format("       a.pedido_numero, "));
                    sql.Append(String.Format("       a.pedido_data_hora, "));
                    sql.Append(String.Format("       a.pedido_observacao, "));
                    sql.Append(String.Format("       a.pedido_tipo_operacao, "));
                    sql.Append(String.Format("       a.pedido_finalizado, "));
                    sql.Append(String.Format("       a.pedido_valor "));
                    sql.Append(String.Format("  from tab_pedido a "));
                    sql.Append(String.Format(" where pedido_finalizado = 1 "));
                    sql.Append(String.Format(" order by a.pedido_data_hora desc"));

                    sdr = _appConn.ExecuteQuery(sql.ToString());

                    while (sdr.Read())
                    {
                        var item = new Pedido
                        {
                            Id = Convert.ToInt64(sdr["pedido_id"].ToString()),
                            Numero = Convert.ToInt64(sdr["pedido_numero"].ToString()),
                            DataHora = (DateTime)(sdr["pedido_data_hora"]),
                            Observacao = sdr["pedido_observacao"].ToString(),
                            //TipoOperacao = (Enumeradores.TipoOperacao)sdr["pedido_tipo_operacao"],
                            //PedidoFinalizado = (bool)(sdr["pedido_finalizado"]),
                            Valor = Convert.ToDouble(sdr["pedido_valor"].ToString())
                        };

                        result.Add(item);
                    }

                    return result;
                }
            }
            catch (Exception)
            { throw; }
        }

        public List<ItemDoPedido> GetAllItensDoPedido(long idPedido)
        {
            StringBuilder sql = new StringBuilder();
            SqlCeDataReader sdr;
            var result = new List<ItemDoPedido>();

            try
            {
                using (_appConn = new AppDbConnection())
                {

                    sql.Append(" select a.pedido_id, ");
                    sql.Append(String.Format("       a.item_id,"));
                    sql.Append(String.Format("       a.item_desc,"));
                    sql.Append(String.Format("       a.grupo_desc,"));
                    sql.Append(String.Format("       a.pedido_item_tamanho,"));
                    sql.Append(String.Format("       a.pedido_quantidade,"));
                    sql.Append(String.Format("       a.pedido_valor_unitario,"));
                    sql.Append(String.Format("       a.pedido_valor_total"));
                    sql.Append(String.Format("  from tab_pedido_item a"));

                    if (idPedido != 0)
                    {
                        sql.Append(String.Format("  where a.pedido_id = {0}", idPedido.ToString()));
                    }

                    sdr = _appConn.ExecuteQuery(sql.ToString());

                    while (sdr.Read())
                    {
                        var item = new ItemDoPedido
                        {
                            IdPedido = Convert.ToInt64(sdr["pedido_id"].ToString()),
                            IdItem = Convert.ToInt64(sdr["item_id"].ToString()),
                            Descricao = sdr["item_desc"].ToString(),
                            Tamanho = sdr["pedido_item_tamanho"].ToString(),
                            DescricaoGrupo = sdr["grupo_desc"].ToString(),
                            QuantidadePedido = Convert.ToInt64(sdr["pedido_quantidade"].ToString()),
                            PrecoVenda = Convert.ToDouble(sdr["pedido_valor_unitario"].ToString()),
                            ValorTotal = Convert.ToDouble(sdr["pedido_valor_total"].ToString())
                        };
                        result.Add(item);
                    }

                    return result;
                }
            }
            catch (Exception)
            { throw; }

        }

        public long PegarProximoNumero()
        {
            StringBuilder sql = new StringBuilder();
            SqlCeDataReader sdr;

            try
            {
                using (_appConn = new AppDbConnection())
                {

                    sql.Append(" select coalesce(max(a.pedido_numero),0) + 1 as proximo");
                    sql.Append(String.Format("  from tab_pedido a"));

                    sdr = _appConn.ExecuteQuery(sql.ToString());

                    if (sdr.Read())
                        return Convert.ToInt64(sdr["proximo"].ToString());
                    else
                        return 1;

                }

            }
            catch (Exception) { throw; }
        }

        public Pedido GetDataById(long idPedido)
        {
            StringBuilder sql = new StringBuilder();
            SqlCeDataReader sdr;
            var result = new Pedido();

            try
            {
                using (_appConn = new AppDbConnection())
                {
                    sql.Append(" select a.pedido_id, ");
                    sql.Append(String.Format("       a.pedido_numero, "));
                    sql.Append(String.Format("       a.pedido_data_hora, "));
                    sql.Append(String.Format("       a.pedido_observacao, "));
                    sql.Append(String.Format("       a.pedido_tipo_operacao, "));
                    sql.Append(String.Format("       a.pedido_finalizado, "));
                    sql.Append(String.Format("       a.pedido_valor "));
                    sql.Append(String.Format("  from tab_pedido a "));
                    sql.Append(String.Format(" where pedido_finalizado = 1 "));
                    sql.Append(String.Format("   and pedido_id = {0}", idPedido.ToString()));

                    sdr = _appConn.ExecuteQuery(sql.ToString());

                    while (sdr.Read())
                    {
                        var item = new Pedido
                        {
                            Id = Convert.ToInt64(sdr["pedido_id"].ToString()),
                            Numero = Convert.ToInt64(sdr["pedido_numero"].ToString()),
                            DataHora = (DateTime)(sdr["pedido_data_hora"]),
                            Observacao = sdr["pedido_observacao"].ToString(),
                            //TipoOperacao = (Enumeradores.TipoOperacao)sdr["pedido_tipo_operacao"],
                            //PedidoFinalizado = (bool)(sdr["pedido_finalizado"]),
                            Valor = Convert.ToDouble(sdr["pedido_valor"].ToString().Replace(".",","))
                        };

                        result = item;
                    }

                    return result;
                }
            }
            catch (Exception)
            { throw; }
        }

    }
}
