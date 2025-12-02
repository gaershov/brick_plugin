// KompasWrapper.cs
using Kompas6API5;
using Kompas6Constants3D;

namespace BrickPlugin.Services
{
    public class KompasWrapper
    {
        private KompasObject _kompas;
        private ksDocument3D _doc3D;
        private ksPart _part;

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

            Thread.Sleep(800);
        }

        public void CreateDocument()
        {
            _doc3D = (ksDocument3D)_kompas.Document3D();
            _doc3D.Create(false, true);

            _part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
        }

        public ksEntity CreateSketchOnXOY()
        {
            ksEntity plane = _part.GetDefaultEntity((short)Obj3dType.o3d_planeXOY);
            ksEntity sketch = _part.NewEntity((short)Obj3dType.o3d_sketch);

            ksSketchDefinition def = (ksSketchDefinition)sketch.GetDefinition();
            def.SetPlane(plane);
            sketch.Create();

            return sketch;
        }

        public ksDocument2D BeginSketch(ksEntity sketch)
        {
            ksSketchDefinition def = (ksSketchDefinition)sketch.GetDefinition();
            return (ksDocument2D)def.BeginEdit();
        }

        public void EndSketch(ksEntity sketch)
        {
            ksSketchDefinition def = (ksSketchDefinition)sketch.GetDefinition();
            def.EndEdit();
        }

        public void Extrude(ksEntity sketch, double h)
        {
            ksEntity extr = _part.NewEntity((short)Obj3dType.o3d_baseExtrusion);
            ksBaseExtrusionDefinition def = (ksBaseExtrusionDefinition)extr.GetDefinition();
            def.SetSketch(sketch);
            ksExtrusionParam p = (ksExtrusionParam)def.ExtrusionParam();
            p.direction = (short)Direction_Type.dtNormal;
            p.typeNormal = (short)End_Type.etBlind;
            p.depthNormal = h;
            extr.Create();
        }

        public void Cut(ksEntity sketch)
        {
            ksEntity op = _part.NewEntity((short)Obj3dType.o3d_cutExtrusion);
            ksCutExtrusionDefinition def = (ksCutExtrusionDefinition)op.GetDefinition();
            def.SetSketch(sketch);
            def.SetSideParam(false, (short)End_Type.etThroughAll, 0, 0, true);
            op.Create();
        }
    }
}
