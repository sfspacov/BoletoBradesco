using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Boleto.Domain;
using Boleto.Infra;
using Newtonsoft.Json;
using Boleto.Domain.Application;
using Boleto.Domain.Entidades;

namespace Boleto.Application.Bradesco
{
    public class BradescoApplication : IBradescoApplication
    {
        #region Private Attributes
        private readonly IEntities _entities;
        private readonly IHttpRequest _httpRequest;
        #endregion

        public BradescoApplication()
        {
            _entities = new Entities();
            _httpRequest = new HttpRequest();
        }

        #region Public Methods
        public string GetUrlBoleto(string orderId)
        {
            var firstOrDefault = _entities.BradescoIntegration.FirstOrDefault(x => x.OrderId == orderId && !x.IsRequest && x.UrlBoleto != null);

            return firstOrDefault != null ? firstOrDefault.UrlBoleto : null;
        }

        public IEnumerable<JsonContent> GetRequests(string document)
        {
            var requests =
                _entities.BradescoIntegration.Where(
                    x => x.IsRequest && x.JsonContent.Contains("\"documento\":\"" + document + "\"")).ToList();

            var jsonContent = new List<JsonContent>();

            foreach (var item in requests)
            {
                var firstOrDefault = _entities.BradescoIntegration.FirstOrDefault(x => x.Token == item.Token && !x.IsRequest);
                var response = string.Empty;
                if (firstOrDefault != null)
                    response = firstOrDefault.JsonContent;

                jsonContent.Add(new JsonContent
                {
                    Requisicao = JsonConvert.DeserializeObject<Requisicao>(item.JsonContent),
                    Resposta = JsonConvert.DeserializeObject<Resposta>(response)
                });
            }

            return jsonContent;
        }

        public bool Check(string numero_pedido, string token)
        {
            var logs = new StringBuilder();
            try
            {
                logs.AppendLine("Bradesco está verificando se a requisição é válida - Método Check (GET)");

                if (string.IsNullOrEmpty(numero_pedido) || string.IsNullOrEmpty(token))
                {
                    logs.AppendLine("ERRO! Parametros inválidos! numero_pedido: '" + numero_pedido + "''; token: '" + token +
                              "'");
                    return false;
                }

                var result = _entities.BradescoIntegration.Any(x => x.OrderId == numero_pedido && x.Token == token);

                logs.AppendLine(!result ? "ERRO! Requisição inválida!" : "Requisição válida!");

                return result;
            }
            catch (Exception e)
            {
                logs.AppendLine("ERRO inesperado! Message: " + e.Message);
                throw;
            }
            finally
            {
                Log.Write(logs.ToString());
            }
        }

        public Resposta Generate(Requisicao requisicao)
        {
            var logs = new StringBuilder();
            logs.AppendLine("Iniciando processo de geração de Boleto do Bradesco.");

            if (requisicao == null)
            {
                logs.AppendLine("ERRO! Requisição nula.");
                throw new ArgumentException("Requisição nula");
            }

            try
            {
                SetFields(requisicao);

                logs.AppendLine("Validando dados...");
                Validate(requisicao);

                logs.AppendLine("Salvando requisição na base de dados...");
                Save(requisicao);

                logs.AppendLine("Iniciando integração com Bradesco (chamada POST)...");
                var resposta = _httpRequest.Post(requisicao);

                logs.AppendLine("Salvando resposta na base de dados...");
                Save(requisicao, resposta);

                if (resposta.status.codigo != "0")
                    throw new Exception("Status: " + resposta.status.codigo + ", Mensagem: " + resposta.status.mensagem);

                logs.AppendLine("Geração de Boleto do Bradesco concluida com sucesso!");
                return resposta;
            }
            catch (Exception e)
            {
                logs.AppendLine("ERRO! Documento comprador: " + requisicao.comprador.documento + " Token da requisição: " +
                            requisicao.token_request_confirmacao_pagamento + "; Mensagem: " + e.Message);
                throw new Exception("Erro ao tentar gerar boleto. Para mais informações leia ao log.");
            }
            finally
            {
                Log.Write(logs.ToString());
            }
        }

        #endregion

        #region Private Methods
        private void Save(Requisicao requisicao, Resposta resposta = null)
        {
            var entity = MapperToEntity(requisicao, resposta);
            _entities.BradescoIntegration.Add(entity);
            _entities.SaveChanges();
        }

        private void SetFields(Requisicao requisicao)
        {
            if (requisicao.comprador.nome.Length > 40)
                requisicao.comprador.nome = requisicao.comprador.nome.Substring(0, 39);

            if (requisicao.comprador.endereco.bairro.Length > 50)
                requisicao.comprador.endereco.bairro = requisicao.comprador.endereco.bairro.Substring(0, 49);

            if (requisicao.comprador.endereco.logradouro.Length > 70)
                requisicao.comprador.endereco.logradouro = requisicao.comprador.endereco.logradouro.Substring(0, 69);

            if (requisicao.comprador.endereco.uf.Length > 2 || string.IsNullOrWhiteSpace(requisicao.comprador.endereco.uf) || string.IsNullOrEmpty(requisicao.comprador.endereco.uf))
                requisicao.comprador.endereco.uf = "..";

            if (requisicao.comprador.endereco.cidade.Length > 50)
                requisicao.comprador.endereco.cidade = requisicao.comprador.endereco.cidade.Substring(0, 49);

            if (requisicao.comprador.endereco.numero.Length > 10)
                requisicao.comprador.endereco.numero = requisicao.comprador.endereco.numero.Substring(0, 9);


            if (requisicao.comprador.endereco.complemento != null && requisicao.comprador.endereco.complemento.Length > 20)
                requisicao.comprador.endereco.complemento = requisicao.comprador.endereco.complemento.Substring(0, 19);

            if (requisicao.comprador.endereco.cep.Contains("-"))
                requisicao.comprador.endereco.cep = requisicao.comprador.endereco.cep.Replace("-", "");

            requisicao.merchant_id = ConfigurationManager.AppSettings["MerchantId"];
            requisicao.token_request_confirmacao_pagamento = Guid.NewGuid().ToString();
            requisicao.meio_pagamento = "300";

            requisicao.boleto.beneficiario = requisicao.boleto.beneficiario ?? "NOME DA SUA EMPRESA";
            requisicao.boleto.carteira = "26";
            requisicao.boleto.nosso_numero = requisicao.pedido.numero.PadRight(11, '0');
            requisicao.boleto.data_emissao = DateTime.Now.ToString("yyyy-MM-dd");
            requisicao.boleto.data_vencimento = requisicao.boleto.data_vencimento ?? DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");
            requisicao.boleto.tipo_renderizacao = "2";

            requisicao.boleto.instrucoes = new Instrucoes
            {
                instrucao_linha_1 = "Não receber valor diferente do impresso em Valor documento.",
                instrucao_linha_2 = "­Caro Usuário:",
                instrucao_linha_3 = "Boleto sujeito às normas vigentes de compensação bancária.",
                instrucao_linha_4 = requisicao.pedido.descricao
            };

            requisicao.boleto.registro.controle_participante = "Segurança arquivo remessa";
            requisicao.boleto.registro.aplicar_multa = false;
            requisicao.boleto.registro.valor_percentual_multa = 0;
            requisicao.boleto.registro.valor_desconto_bonificacao = 0;
            requisicao.boleto.registro.debito_automatico = false;
            requisicao.boleto.registro.rateio_credito = false;
            requisicao.boleto.registro.endereco_debito_automatico = "2";
            requisicao.boleto.registro.especie_titulo = "99";
            requisicao.boleto.registro.primeira_instrucao = "00";
            requisicao.boleto.registro.segunda_instrucao = "00";
            requisicao.boleto.registro.valor_juros_mora = 0;
            requisicao.boleto.registro.valor_desconto = 0;
            requisicao.boleto.registro.tipo_ocorrencia = "01";
            requisicao.boleto.registro.tipo_inscricao_pagador = "01";

            var sequencial = "1";

            if (_entities.BradescoIntegration.Any())
                sequencial = (_entities.BradescoIntegration.OrderByDescending(x => x.Id).Take(1).Single().Id + 1).ToString();

            requisicao.boleto.registro.sequencia_registro = sequencial.PadLeft(5, '0');
            requisicao.comprador.endereco.cep = requisicao.comprador.endereco.cep.Replace("-", "");
            requisicao.comprador.documento = requisicao.comprador.documento.Replace(".", "").Replace("-", "").Replace("/", "");
            requisicao.pedido.valor = requisicao.pedido.valor.Replace(".", "").Replace(",", "");
            requisicao.boleto.valor_titulo = requisicao.boleto.valor_titulo.Replace(".", "").Replace(",", "");
        }

        private static BradescoIntegration MapperToEntity(Requisicao requisicao, Resposta resposta = null)
        {
            var entity = new BradescoIntegration
            {
                CreateDate = DateTime.Now,
                IsRequest = resposta == null,
                OrderId = requisicao.pedido.numero,
                Token = requisicao.token_request_confirmacao_pagamento,
                JsonContent = resposta == null ? JsonConvert.SerializeObject(requisicao) : JsonConvert.SerializeObject(resposta),
                UrlBoleto = resposta == null ? null : (resposta.boleto == null ? null : resposta.boleto.url_acesso)
            };

            return entity;
        }

        private static void Validate(Requisicao requisicao)
        {
            var conditions = new Dictionary<string, bool>
            {
                   {"Campo obrigatório não preenchido!",
                !RequiredFields(requisicao)},
                   {"Alguma data está inválida!",
                !DateFields(requisicao)},
                   {"MerchantId deve conter 9 caracteres!",
                requisicao.merchant_id.Length != 9},
                   {"Meio de pagamento deve ser '300'!",
                requisicao.meio_pagamento != "300"},
                   {"Numero do pedido deve conter no máximo 27 caracteres!",
                requisicao.pedido.numero.Length > 27},
                   {"Valor do pedido deve conter no máximo 13 caracteres!",
                requisicao.pedido.valor.Length > 13},
                   {"Descrição do pedido deve conter no máximo 255 caracteres!",
                requisicao.pedido.descricao.Length > 255},
                   {"Nome do comprador deve conter no máximo 40 caracteres!",
                requisicao.comprador.nome.Length > 40},
                   {"Documento do comprador deve conter entre 11 e 14 caracteres!",
                requisicao.comprador.documento.Length < 11 || requisicao.comprador.documento.Length > 14},
                   {"IP do comprador deve conter entre 9 e 50 caracteres!",
                !string.IsNullOrEmpty(requisicao.comprador.ip) && (requisicao.comprador.ip.Length < 9 || requisicao.comprador.ip.Length > 50)},
                   {"User Agent do comprador deve conter no máximo 255 caracteres!",
                !string.IsNullOrEmpty(requisicao.comprador.user_agent) && requisicao.comprador.user_agent.Length > 255},
                   {"CEP do comprador deve conter 8 caracteres!",
                requisicao.comprador.endereco.cep.Length != 8},
                   {"Logradouro do comprador deve conter no máximo 70 caracteres!",
                requisicao.comprador.endereco.logradouro.Length > 70},
                   {"Numero do Endereço comprador deve conter no máximo 10 caracteres!",
                requisicao.comprador.endereco.numero.Length > 10},
                   {"Complemento do Endereço do comprador deve conter no máximo 20 caracteres!",
                !string.IsNullOrEmpty(requisicao.comprador.endereco.complemento) && requisicao.comprador.endereco.complemento.Length > 20},
                   {"Bairro do comprador deve conter no máximo 50 caracteres!",
                requisicao.comprador.endereco.bairro.Length > 50},
                   {"Cidade do comprador deve conter no máximo 50 caracteres!",
                requisicao.comprador.endereco.cidade.Length > 50},
                   {"UF do comprador deve conter 2 caracteres!",
                requisicao.comprador.endereco.uf.Length != 2},
                   {"Beneficiário do boleto deve conter no máximo 150 caracteres!",
                requisicao.boleto.beneficiario.Length > 150},
                   {"Nosso número deve conter 11 caracteres!",
                requisicao.boleto.nosso_numero.Length != 11},
                   {"Carteira deve conter 2 caracteres!",
                requisicao.boleto.carteira.Length != 2},
                   {"Data de emissão deve conter 10 caracteres!",
                requisicao.boleto.data_emissao.Length != 10},
                   {"Data de vencimento deve conter 10 caracteres!",
                requisicao.boleto.data_vencimento.Length != 10},
                   {"Valor do título deve conter no máximo 13 caracteres!",
                requisicao.boleto.valor_titulo.Length > 13},
                   {"URL do logotipo deve conter no máximo 255 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.url_logotipo) && requisicao.boleto.url_logotipo.Length > 255},
                   {"Mensagem cabeçalho deve conter no máximo 255 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.mensagem_cabecalho) && requisicao.boleto.mensagem_cabecalho.Length > 255},
                   {"Tipo renderização deve conter no máximo 1 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.tipo_renderizacao) && requisicao.boleto.tipo_renderizacao.Length > 1},
                   {"Instrução linha 1 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_1) && requisicao.boleto.instrucoes.instrucao_linha_1.Length > 60},
                   {"Instrução linha 2 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_2) && requisicao.boleto.instrucoes.instrucao_linha_2.Length > 60},
                   {"Instrução linha 3 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_3) && requisicao.boleto.instrucoes.instrucao_linha_3.Length > 60},
                   {"Instrução linha 4 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_4) && requisicao.boleto.instrucoes.instrucao_linha_4.Length > 60},
                   {"Instrução linha 5 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_5) && requisicao.boleto.instrucoes.instrucao_linha_5.Length > 60},
                   {"Instrução linha 6 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_6) && requisicao.boleto.instrucoes.instrucao_linha_6.Length > 60},
                   {"Instrução linha 7 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_7) && requisicao.boleto.instrucoes.instrucao_linha_7.Length > 60},
                   {"Instrução linha 8 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_8) && requisicao.boleto.instrucoes.instrucao_linha_8.Length > 60},
                   {"Instrução linha 9 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_9) && requisicao.boleto.instrucoes.instrucao_linha_9.Length > 60},
                   {"Instrução linha 10 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_10) && requisicao.boleto.instrucoes.instrucao_linha_10.Length > 60},
                   {"Instrução linha 11 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_11) && requisicao.boleto.instrucoes.instrucao_linha_11.Length > 60},
                   {"Instrução linha 12 deve conter no máximo 60 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.instrucoes.instrucao_linha_12) && requisicao.boleto.instrucoes.instrucao_linha_12.Length > 60},
                   {"Registro, Agencia pagador, deve conter no máximo 5 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.agencia_pagador) && requisicao.boleto.registro.agencia_pagador.Length > 5},
                   {"Registro, Razão conta pagador, deve conter no máximo 5 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.razao_conta_pagador) && requisicao.boleto.registro.razao_conta_pagador.Length > 5},
                   {"Registro, Conta pagador, deve conter no máximo 8 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.conta_pagador) && requisicao.boleto.registro.conta_pagador.Length > 8},
                   {"Registro, Controle participante, deve conter no máximo 25 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.controle_participante) && requisicao.boleto.registro.controle_participante.Length > 25},
                   {"Registro, Valor percentual multa, deve conter no máximo 4 caracteres!",
                requisicao.boleto.registro.valor_percentual_multa > 0 && requisicao.boleto.registro.valor_percentual_multa.ToString().Length > 4},
                   {"Registro, Valor desconto bonificação, deve conter no máximo 10 caracteres!",
                requisicao.boleto.registro.valor_desconto_bonificacao > 0 && requisicao.boleto.registro.valor_percentual_multa.ToString().Length > 10},
                   {"Registro, Endereço débito automático, deve conter no máximo 1 caracter!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.endereco_debito_automatico) && requisicao.boleto.registro.endereco_debito_automatico.Length > 1},
                   {"Registro, Tipo ocorrência, deve conter no máximo 3 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.tipo_ocorrencia) && requisicao.boleto.registro.tipo_ocorrencia.Length > 3},
                   {"Registro, Espécie título, deve conter no máximo 2 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.especie_titulo) && requisicao.boleto.registro.especie_titulo.Length > 2},
                   {"Registro, Primeira instrução, deve conter no máximo 2 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.primeira_instrucao) && requisicao.boleto.registro.primeira_instrucao.Length > 2},
                   {"Registro, Segunda instrução, deve conter no máximo 2 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.segunda_instrucao) && requisicao.boleto.registro.segunda_instrucao.Length > 2},
                   {"Registro, Valor juros mora, deve conter no máximo 13 caracteres!",
                requisicao.boleto.registro.valor_juros_mora > 0 && requisicao.boleto.registro.valor_juros_mora.ToString().Length > 13},
                   {"Registro, Data limite concessão desconto, deve conter no máximo 10 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.data_limite_concessao_desconto) && requisicao.boleto.registro.segunda_instrucao.Length > 10},
                   {"Registro, Valor desconto, deve conter no máximo 13 caracteres!",
                requisicao.boleto.registro.valor_desconto > 0 && requisicao.boleto.registro.valor_desconto.ToString().Length > 13},
                   {"Registro, Valor IOF, deve conter no máximo 13 caracteres!",
                requisicao.boleto.registro.valor_iof > 0 && requisicao.boleto.registro.valor_iof.ToString().Length > 13},
                   {"Registro, Valor abatimento, deve conter no máximo 13 caracteres!",
                requisicao.boleto.registro.valor_abatimento > 0 && requisicao.boleto.registro.valor_abatimento.ToString().Length > 13},
                   {"Registro, Tipo inscrição pagador, deve conter no máximo 2 caracteres!",
                !string.IsNullOrEmpty(requisicao.boleto.registro.tipo_inscricao_pagador) && requisicao.boleto.registro.tipo_inscricao_pagador.Length > 2},
                   {"Token deve conter no máximo 256 caracteres!",
                requisicao.token_request_confirmacao_pagamento.Length > 256},
            };

            var invalidConditions = conditions.Where(condition => condition.Value).ToList();

            foreach (var condition in invalidConditions)
                throw new Exception(condition.Key);
        }

        private static bool DateFields(Requisicao requisicao)
        {
            var dates = new List<string>
            {
                requisicao.boleto.data_emissao,
                requisicao.boleto.data_vencimento,
                requisicao.boleto.registro.data_limite_concessao_desconto,
            };

            foreach (var date in dates.Where(date => !string.IsNullOrEmpty(date)))
            {
                DateTime outVariable;
                var finalResult = DateTime.TryParse(date, out outVariable);
                if (!finalResult)
                    return false;
            }

            return true;
        }

        private static bool RequiredFields(Requisicao requisicao)
        {
            if (string.IsNullOrEmpty(requisicao.merchant_id) ||
                string.IsNullOrEmpty(requisicao.meio_pagamento) ||
                string.IsNullOrEmpty(requisicao.pedido.valor) ||
                string.IsNullOrEmpty(requisicao.pedido.descricao) ||
                string.IsNullOrEmpty(requisicao.comprador.nome) ||
                string.IsNullOrEmpty(requisicao.comprador.documento) ||
                string.IsNullOrEmpty(requisicao.comprador.endereco.cep) ||
                string.IsNullOrEmpty(requisicao.comprador.endereco.logradouro) ||
                string.IsNullOrEmpty(requisicao.comprador.endereco.numero) ||
                string.IsNullOrEmpty(requisicao.comprador.endereco.bairro) ||
                string.IsNullOrEmpty(requisicao.comprador.endereco.cidade) ||
                string.IsNullOrEmpty(requisicao.comprador.endereco.uf) ||
                string.IsNullOrEmpty(requisicao.boleto.beneficiario) ||
                string.IsNullOrEmpty(requisicao.boleto.carteira) ||
                string.IsNullOrEmpty(requisicao.boleto.nosso_numero) ||
                string.IsNullOrEmpty(requisicao.boleto.data_emissao) ||
                string.IsNullOrEmpty(requisicao.boleto.data_vencimento) ||
                string.IsNullOrEmpty(requisicao.boleto.valor_titulo) ||
                string.IsNullOrEmpty(requisicao.token_request_confirmacao_pagamento)
                )
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}