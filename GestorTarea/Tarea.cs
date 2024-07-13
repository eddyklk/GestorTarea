using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorTarea
{
    public class Tarea
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Prioridad { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Categoria { get; set; }

        public Tarea(string titulo, string descripcion, int prioridad, DateTime fechaVencimiento, string categoria)
        {
            Titulo = titulo;
            Descripcion = descripcion;
            Prioridad = prioridad;
            FechaVencimiento = fechaVencimiento;
            Categoria = categoria;
        }

        public override string ToString()
        {
            return $"{Titulo}: {Descripcion} (Prioridad: {Prioridad}, Vence: {FechaVencimiento.ToShortDateString()}, Categoría: {Categoria})";
        }
    }
}
