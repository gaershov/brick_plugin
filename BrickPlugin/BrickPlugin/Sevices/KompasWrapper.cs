using Kompas6API5;
using Kompas6Constants3D;

namespace BrickPlugin.Services
{
    /// <summary>
    /// Обертка для работы с API КОМПАС-3D.
    /// </summary>
    public class KompasWrapper
    {
        /// <summary>
        /// Объект приложения КОМПАС.
        /// </summary>
        private KompasObject _kompas;

        /// <summary>
        /// 3D документ КОМПАС.
        /// </summary>
        private ksDocument3D _doc3D;

        /// <summary>
        /// Деталь в 3D документе.
        /// </summary>
        private ksPart _part;

        /// <summary>
        /// Открывает или активирует приложение КОМПАС-3D.
        /// </summary>
        /// <exception cref="Exception">
        /// Выбрасывается, если КОМПАС не установлен в системе.
        /// </exception>
        public void OpenKompas()
        {
            if (_kompas != null)
            {
                return;
            }
            Type t = Type.GetTypeFromProgID("KOMPAS.Application.23")
                ?? Type.GetTypeFromProgID("KOMPAS.Application.22")
                ?? Type.GetTypeFromProgID("Kompas.Application.5");
            if (t == null)
            {
                throw new Exception("КОМПАС не установлен");
            }

            _kompas = (KompasObject)Activator.CreateInstance(t);
            _kompas.Visible = true;
            _kompas.ActivateControllerAPI();
        }

        /// <summary>
        /// Создает новый 3D документ в КОМПАС.
        /// </summary>
        public void CreateDocument()
        {
            _doc3D = (ksDocument3D)_kompas.Document3D();
            _doc3D.Create(false, true);

            _part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
        }

        /// <summary>
        /// Создает эскиз на плоскости XOY.
        /// </summary>
        /// <returns>Созданный эскиз.</returns>
        public ksEntity CreateSketchOnXOY()
        {
            ksEntity plane = _part.GetDefaultEntity(
                (short)Obj3dType.o3d_planeXOY);
            ksEntity sketch = _part.NewEntity((short)Obj3dType.o3d_sketch);

            ksSketchDefinition def =
                (ksSketchDefinition)sketch.GetDefinition();
            def.SetPlane(plane);
            sketch.Create();

            return sketch;
        }

        /// <summary>
        /// Начинает редактирование эскиза.
        /// </summary>
        /// <param name="sketch">Эскиз для редактирования.</param>
        /// <returns>2D документ для работы с эскизом.</returns>
        public ksDocument2D BeginSketch(ksEntity sketch)
        {
            ksSketchDefinition def =
                (ksSketchDefinition)sketch.GetDefinition();
            return (ksDocument2D)def.BeginEdit();
        }

        /// <summary>
        /// Завершает редактирование эскиза.
        /// </summary>
        /// <param name="sketch">Эскиз для завершения редактирования.</param>
        public void EndSketch(ksEntity sketch)
        {
            ksSketchDefinition def =
                (ksSketchDefinition)sketch.GetDefinition();
            def.EndEdit();
        }

        /// <summary>
        /// Выполняет операцию выдавливания.
        /// </summary>
        /// <param name="sketch">Эскиз для выдавливания.</param>
        /// <param name="height">Высота выдавливания.</param>
        public void Extrude(ksEntity sketch, double height)
        {
            ksEntity extr = _part.NewEntity(
                (short)Obj3dType.o3d_baseExtrusion);
            ksBaseExtrusionDefinition def =
                (ksBaseExtrusionDefinition)extr.GetDefinition();
            def.SetSketch(sketch);
            ksExtrusionParam p = (ksExtrusionParam)def.ExtrusionParam();
            p.direction = (short)Direction_Type.dtNormal;
            p.typeNormal = (short)End_Type.etBlind;
            p.depthNormal = height;
            extr.Create();
        }

        /// <summary>
        /// Выполняет операцию вырезания.
        /// </summary>
        /// <param name="sketch">Эскиз для вырезания.</param>
        public void Cut(ksEntity sketch)
        {
            ksEntity op = _part.NewEntity(
                (short)Obj3dType.o3d_cutExtrusion);
            ksCutExtrusionDefinition def =
                (ksCutExtrusionDefinition)op.GetDefinition();
            def.SetSketch(sketch);
            def.SetSideParam(false,(short)End_Type.etThroughAll, 0, 0, true);
            op.Create();
        }

        /// <summary>
        /// Закрывает текущий документ без сохранения.
        /// </summary>
        public void CloseDocument()
        {
            if (_doc3D != null)
            {
                _doc3D.close();
                _doc3D = null;
                _part = null;
            }
        }
    }
}