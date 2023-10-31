namespace APICatalogo.Services
{
    public class MeuServico : IMeuServico
    {
        public string GetSaudacao(string nome)
        {
            return $"Olá, {nome} \n\n {DateTime.Now}";
        }
    }
}
