using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorTarea
{
   

        public class GestorTareas
        {
            private List<Tarea> tareas = new List<Tarea>();
            private Stack<string> historialAcciones = new Stack<string>();
            private Stack<string> pilaRehacer = new Stack<string>();
            private Queue<Tarea> tareasUrgentes = new Queue<Tarea>();
            private Dictionary<string, List<Tarea>> arbolTareas = new Dictionary<string, List<Tarea>>();
            private Dictionary<string, Tarea> respaldoTareas = new Dictionary<string, Tarea>();

            public void AgregarTarea(Tarea tarea)
            {
                tareas.Add(tarea);
                tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
                historialAcciones.Push($"Agregar:{tarea.Titulo}");
                AgregarTareaAlArbol(tarea);
                respaldoTareas[tarea.Titulo] = tarea;
            }

            public void EliminarTarea(string titulo)
            {
                Tarea tarea = tareas.Find(t => t.Titulo == titulo);
                if (tarea != null)
                {
                    tareas.Remove(tarea);
                    historialAcciones.Push($"Eliminar:{tarea.Titulo}");
                    EliminarTareaDelArbol(tarea);
                    respaldoTareas[tarea.Titulo] = tarea;
                }
            }

            public void ModificarTarea(string titulo, Tarea nuevaTarea)
            {
                Tarea tareaOriginal = tareas.Find(t => t.Titulo == titulo);
                if (tareaOriginal != null)
                {
                    respaldoTareas[titulo] = tareaOriginal;
                    EliminarTarea(titulo);
                    AgregarTarea(nuevaTarea);
                    historialAcciones.Push($"Modificar:{titulo}");
                }
            }

            public void MostrarTareas()
            {
                foreach (var tarea in tareas)
                {
                    Console.WriteLine(tarea);
                }
            }

            public void AgregarTareaUrgente(Tarea tarea)
            {
                tareasUrgentes.Enqueue(tarea);
                historialAcciones.Push($"AgregarUrgente:{tarea.Titulo}");
                respaldoTareas[tarea.Titulo] = tarea;
            }

            public void ProcesarTareasUrgentes()
            {
                while (tareasUrgentes.Count > 0)
                {
                    Tarea tarea = tareasUrgentes.Dequeue();
                    Console.WriteLine($"Procesando tarea urgente: {tarea}");
                }
            }

            private void AgregarTareaAlArbol(Tarea tarea)
            {
                if (!arbolTareas.ContainsKey(tarea.Categoria))
                {
                    arbolTareas[tarea.Categoria] = new List<Tarea>();
                }
                arbolTareas[tarea.Categoria].Add(tarea);
            }

            private void EliminarTareaDelArbol(Tarea tarea)
            {
                if (arbolTareas.ContainsKey(tarea.Categoria))
                {
                    arbolTareas[tarea.Categoria].Remove(tarea);
                }
            }

            public void MostrarArbolTareas()
            {
                foreach (var categoria in arbolTareas.Keys)
                {
                    Console.WriteLine($"{categoria}:");
                    foreach (var tarea in arbolTareas[categoria])
                    {
                        Console.WriteLine($"\t{tarea}");
                    }
                }
            }

            public void Deshacer()
            {
                if (historialAcciones.Count > 0)
                {
                    string ultimaAccion = historialAcciones.Pop();
                    string[] partesAccion = ultimaAccion.Split(':');
                    string tipoAccion = partesAccion[0];
                    string tituloTarea = partesAccion[1];

                    switch (tipoAccion)
                    {
                        case "Agregar":
                            Tarea tareaAgregada = respaldoTareas[tituloTarea];
                            tareas.Remove(tareaAgregada);
                            EliminarTareaDelArbol(tareaAgregada);
                            pilaRehacer.Push(ultimaAccion);
                            break;
                        case "Eliminar":
                            Tarea tareaEliminada = respaldoTareas[tituloTarea];
                            tareas.Add(tareaEliminada);
                            AgregarTareaAlArbol(tareaEliminada);
                            pilaRehacer.Push(ultimaAccion);
                            break;
                        case "Modificar":
                            Tarea tareaOriginal = respaldoTareas[tituloTarea];
                            Tarea tareaModificada = tareas.Find(t => t.Titulo == tituloTarea);
                            if (tareaModificada != null)
                            {
                                tareas.Remove(tareaModificada);
                                tareas.Add(tareaOriginal);
                                tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
                                EliminarTareaDelArbol(tareaModificada);
                                AgregarTareaAlArbol(tareaOriginal);
                                pilaRehacer.Push(ultimaAccion);
                            }
                            break;
                        case "AgregarUrgente":
                            Tarea tareaUrgente = respaldoTareas[tituloTarea];
                            tareasUrgentes = new Queue<Tarea>(tareasUrgentes.Where(t => t.Titulo != tituloTarea));
                            pilaRehacer.Push(ultimaAccion);
                            break;
                    }
                }
            }

            public void Rehacer()
            {
                if (pilaRehacer.Count > 0)
                {
                    string ultimaAccionDeshecha = pilaRehacer.Pop();
                    string[] partesAccion = ultimaAccionDeshecha.Split(':');
                    string tipoAccion = partesAccion[0];
                    string tituloTarea = partesAccion[1];

                    Tarea tarea = respaldoTareas[tituloTarea];

                    switch (tipoAccion)
                    {
                        case "Agregar":
                            tareas.Add(tarea);
                            tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
                            AgregarTareaAlArbol(tarea);
                            break;
                        case "Eliminar":
                            tareas.Remove(tarea);
                            EliminarTareaDelArbol(tarea);
                            break;
                        case "Modificar":
                            Tarea tareaModificada = tareas.Find(t => t.Titulo == tarea.Titulo);
                            if (tareaModificada != null)
                            {
                                tareas.Remove(tareaModificada);
                                tareas.Add(tarea);
                                tareas.Sort((x, y) => x.Prioridad == y.Prioridad ? x.FechaVencimiento.CompareTo(y.FechaVencimiento) : x.Prioridad.CompareTo(y.Prioridad));
                                EliminarTareaDelArbol(tareaModificada);
                                AgregarTareaAlArbol(tarea);
                            }
                            break;
                        case "AgregarUrgente":
                            tareasUrgentes.Enqueue(tarea);
                            break;
                    }
                    historialAcciones.Push(ultimaAccionDeshecha);
                }
            }
        




    }
}
