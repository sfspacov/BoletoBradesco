using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Boleto.Domain.Application;
using Boleto.Domain.Entidades;
using Boleto.Application.Bradesco;

namespace LevelUp.Boleto.Test
{
    [TestClass]
    public class BradescoApplicationTest
    {
        private readonly IBradescoApplication _bradescoApplication = new BradescoApplication();

        #region Public Methods

        #region Check

        [TestMethod]
        public void Check_NumeroPedidoInvalido_False()
        {
            var result = _bradescoApplication.Check("0", "65af91aa-631b-48f9-a552-29ef33e837ef");
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Check_TokenInvalido_False()
        {
            var result = _bradescoApplication.Check("74976085", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Check_RequisicaoInvalida_False()
        {
            var result = _bradescoApplication.Check("74976085", "xpto");
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Check_RequisicaoOk_True()
        {
            //Base de produção
            var result = _bradescoApplication.Check("74976085", "65af91aa-631b-48f9-a552-29ef33e837ef");
            Assert.AreEqual(true, result);
        }

        #endregion

        #region Generate

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Generate_RequisicaoNula_Exception()
        {
            _bradescoApplication.Generate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Generate_CamposObrigatoriosNaoPreenchidos_Exception()
        {
            var requisicao = FakeRequisicao();
            requisicao.pedido.valor = "0";
            _bradescoApplication.Generate(requisicao);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Generate_DataInvalida_Exception()
        {
            var requisicao = FakeRequisicao();
            requisicao.boleto.registro.data_limite_concessao_desconto = "a";
            _bradescoApplication.Generate(requisicao);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Generate_ValidandoLength_Exception()
        {
            var requisicao = FakeRequisicao();
            requisicao.comprador.nome =
                "tghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gftghgbhgn5fg5n9+g55g9h5gd9+h5fd9+h459sdhsaidhsaidhsaiduhsauidhsauidhsauidhsauidhsauidhsauidhsauidhsauihdsauihduisahdusahduisahduisahduisahdusahdusahduiahjnsdj h diosa hdisa duisah iu hsadiosa dsa84hg84hgf84hgf84hgf8h4gf8h4gf8h4gf";
            _bradescoApplication.Generate(requisicao);
        }

        [TestMethod]
        public void Generate_RequestOk_ReturnReposta()
        {
            var requisicao = FakeRequisicao();
            var rnd = new Random();
            var myRandomNo = rnd.Next(10000000, 99999999);
            requisicao.pedido.numero = myRandomNo.ToString();
            var result = _bradescoApplication.Generate(requisicao);
            Assert.AreEqual(result.status.codigo, "0");
        }

        #endregion

        #endregion

        #region Private Methods

        private static Requisicao FakeRequisicao()
        {
            return new Requisicao
            {
                pedido = new Pedido
                {
                    numero = "74342844",
                    descricao = "GTA V",
                    valor = "2500"
                },
                comprador = new Comprador
                {
                    nome = "João da Silva",
                    documento = "84941890264",
                    endereco = new Endereco
                    {
                        uf = "AM",
                        cidade = "Itacoatiara",
                        bairro = "Jardim Florestal",
                        logradouro = "Rua Angelim",
                        cep = "69101627",
                        numero = "2899"
                    },
                    ip = "192.168.41.143",
                    user_agent = "Google Chrome",
                },
                boleto = new BoletoRequest()
                {
                    valor_titulo = "1990",
                    registro = new Registro()
                },
            };
        }

        #endregion
    }
}
