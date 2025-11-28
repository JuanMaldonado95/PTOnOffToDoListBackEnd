using System.Text;

namespace WebApiPTBackOnOff.Shared.Utils
{
    public class GenericUtils
    {
        public static async Task Log(string detail, Exception err = null)
        {
            StringBuilder sb = new StringBuilder()
                .Append("##################################################").AppendLine()
                .Append($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} - {detail}").AppendLine()
                .Append("##################################################").AppendLine();

            while (err != null)
            {
                sb.Append(err.Message)
                    .AppendLine()
                    .Append(err.StackTrace)
                    .AppendLine();

                err = err.InnerException;

                if (err != null)
                    sb.Append("--------------------------------------------------").AppendLine();
            }

            string file = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(file);
            await File.AppendAllTextAsync(Path.Combine(file, "error.log"), sb.ToString());
        }
    }
}
