using Matrix;
using Matrix.Xmpp.Register;
using System.Threading.Tasks;

namespace NotificationService.XMPP
{
    public class RegisterAccountHandler : IRegister
    {
        private XmppClient xmppClient;
        public RegisterAccountHandler(XmppClient xmppClient)
        {
            this.xmppClient = xmppClient;
        }

        public bool RegisterNewAccount => true;

        public async Task<Register> RegisterAsync(Register register)
        {
            register.Username = xmppClient.Username;
            register.Password = xmppClient.Password;
            return register;
        }
    }
}
