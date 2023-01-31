using Microsoft.Extensions.Logging;
using System;

namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        void RealizarTarea();
        
    }
    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;

        public ServicioA(ILogger<ServicioA> logger)
        {
            this.logger = logger;
        }
        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }

    public class ServicioB : IServicio
    {
        private readonly ILogger<ServicioA> logger;

        public ServicioB(ILogger<ServicioA> logger)
        {
            this.logger = logger;
        }
        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }
}
