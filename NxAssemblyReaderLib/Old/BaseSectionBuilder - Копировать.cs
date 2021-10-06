using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VertexCodeMakerDomain;
using VertexCodeMakerDomain.Interfaces;

namespace NxAssemblyReaderLib
{
    class BaseSectionBuilderOld
    {
       
        public List<TemplateSection> Build(BasePart[] parts, Dictionary<Operations, double> servicesList, string assemblyName)
        {
            List<TemplateSection> bases = new List<TemplateSection>();
            
            foreach (var item in parts)
            {
                // ignore Assembly part
                if (assemblyName != item.Name)
                {
                    bases.Add(BuildBaseSecionsBy(item, servicesList));
                }
            }
            return bases;
        }
        private TemplateSection BuildBaseSecionsBy(BasePart part, Dictionary<Operations, double> servicesList)
        {
            var retVal = new Dictionary<string, ISection>();
            
            BaseAttributes bases = GeteSizeAttributeFrom(GetContourFlangeFrom(part));

            // Get commands and sort by ordinate distance
            var vertexCommands = GetCommandsFrom(part, bases.Width, servicesList).OrderBy(x => x.Ordinate).ToList();
            return new TemplateSection(part.Name, vertexCommands, bases.Width, bases.Hight, bases.Length, bases.Thickness);
        }
        private List<VertexCommand> GetCommandsFrom(BasePart part, double partWidh, Dictionary<Operations, double> servicesList)
        {
            List<VertexCommand> vertexCommands = new List<VertexCommand>();
            vertexCommands.AddRange(GetDimpleCommandsFrom(part).Select(x=>x.Clone() as VertexCommand));
            vertexCommands.AddRange(GetCutoutCommandsFrom(part, partWidh).Select(x => x.Clone() as VertexCommand));
            vertexCommands.AddRange(GetServiceCommandsFrom(part, servicesList).Select(x => x.Clone() as VertexCommand));
            //vertexCommands.AddRange(AddAdditionalRules(vertexCommands, partWidh, part.Name));
            return vertexCommands;
        }
        //private List<VertexCommand> AddAdditionalRules(List<VertexCommand> vertexCommands, double width, string partName)
        //{
        //    List<VertexCommand> commands = new List<VertexCommand>();
        //    // If swage already added => break
        //    if (vertexCommands.Any(x => x.Operation == Operations.Swage))
        //        return commands;

        //    // Rule 1
        //    var b = vertexCommands.GroupBy(x => x.Ordinate).Where(y => y.Count() > 1);
        //    foreach (var item in b)
        //    {
        //        var c  = item.ToArray();
        //        // Notch and lip
        //        if (c.Length == 2 && c.Any(x => x.Operation == Operations.Notch) && c.Any(x => x.Operation == Operations.Lip))
        //        {
        //            commands.Add(new VertexCommand(partName, Operations.Swage, 15));
        //        }
        //    }
        //    // If swage already added then return
        //    // Rule 2
        //    if (commands.Count != 0)
        //    {
        //        return commands;
        //    }
        //    int counter = 0;
        //    foreach (var item in vertexCommands)
        //    {
        //        if (counter == 3)
        //        {
        //            commands.Add(new VertexCommand(partName, Operations.Swage, 15));
        //        }
        //        if (item.Operation == Operations.Notch)
        //        {
        //            counter++;
        //        }
        //        else
        //        {
        //            counter = 0;
        //        }
        //    }
        //    return commands;
        //}
        private List<VertexCommand> GetDimpleCommandsFrom(BasePart part)
        {
            var commands = new List<VertexCommand>();
            //var features = FindFeaturesBy<Dimple>(part);
            var features = FindFeaturesBy<Dimple>(part, "Dimple");
            // if part dont contain dimples - return list without elements
            if (features == null || features.Count == 0)
            {
                return commands;
            }
            foreach (var feature in features)
            {
                // Take geoms from sketch
                Arc arc = GetGeometryFrom(feature, 1).Select(x => CastNxObjectTo<Arc>(x)).First();
                // Get X axis value from casted Arc and create vertex command
                commands.Add(new VertexCommand(part.Name, Operations.Dimple, arc.CenterPoint.X));
            }
            // Error 
            if (features.Count != commands.Count)
            {
                throw new Exception($"Some dimples cant finded, input amount - {features.Count}, output amount - {commands.Count}");
            }
            return commands.GroupBy(x => x.Ordinate).Select(y => y.First()).ToList();
        }
        private List<VertexCommand> GetCutoutCommandsFrom(BasePart part, double width)
        {
            // TODO: FIX THIS
            int tempboost = 0;
            var commands = new List<VertexCommand>();
            //var features = FindFeaturesBy<NormalCutout>(part);
            var features = FindFeaturesBy<NormalCutout>(part, "Normal Cutout");
            // if part dont contain cutouts return - list without elements
            if (features == null || features.Count == 0)
            {
                return commands;
            }
            foreach (var feature in features)
            {
                Operations operation = UserAttributeControlBy(feature);
                // Error
                if (operation == Operations.None)
                {
                    throw new Exception($"feature \"{feature.Name}\" in {part.Name} dont have detection attribute");
                }
                // Take geoms from sketch
                Line[] lines = GetGeometryFrom(feature).Select(x=>CastNxObjectTo<Line>(x)).ToArray();
                if (operation == Operations.Notch)
                {
                    commands.Add(CreateNotchCommand(lines, part.Name));
                }
                else if (operation == Operations.Lip)
                {
                    commands.Add(CreateLipCommand(lines, part.Name));
                }
                else if (operation == Operations.Chamfer && !commands.Exists(x=>x.Operation == Operations.Chamfer))
                {
                    commands.AddRange(CreateChamferCommands(width, part.Name));
                    tempboost += 2;
                }
                // Get X axis value from casted Arc and create vertex command
            }
            // Error 
            if (features.Count + tempboost != commands.Count)
            {
                throw new Exception($"Some cutouts cant finded, input amount - {features.Count}, output amount - {commands.Count}");
            }
            return commands.GroupBy(x=>x.Ordinate).Select(y=>y.First()).ToList();
        }
        private List<VertexCommand> GetServiceCommandsFrom(BasePart part, Dictionary<Operations, double> servicesList)
        {
            var commands = new List<VertexCommand>();
            // var features = FindFeaturesBy<HolePackage>(part);
            var features = FindFeaturesBy<HolePackage>(part, "HOLE PACKAGE");
            // if part dont contain cutouts return - list without elements
            if (features == null || features.Count == 0)
            {
                return commands;
            }
            var holes = features.Select(x => CastNxObjectTo<HolePackage>(x)).ToArray();
            commands = holes.Select(x => CreateService(x, servicesList, part.Name)).ToList();
            if (features.Count != commands.Count)
            {
                throw new Exception($"Some services cant finded, input amount - {features.Count}, output amount - {commands.Count}");
            }
            return commands.GroupBy(x => x.Ordinate).Select(y => y.First()).ToList();
        }
        private ContourFlange GetContourFlangeFrom(BasePart part)
        {
            try
            {
                return part.Features.ToArray().Where(x => x.FeatureType == "BCFLANGE").First() as ContourFlange;
            }
            catch (Exception)
            {
                throw new Exception($"part \"{part.Name}\" dont contain feature BCFLANGE");
            }
        }
        private VertexCommand CreateService(HolePackage hole, Dictionary<Operations,double> servicesList, string partName)
        {
            // TODO: add determinition vector
            var diameter = GetDiameterFrom(hole);
            Operations op;
            try
            {
                op = servicesList.Where(x => x.Value == diameter).First().Key;
            }
            catch (Exception)
            {

                throw new Exception($"Service \"{hole.Name}\" have wrong diameter");
            }
            hole.GetDirections(out Vector3d[] vectors);
            hole.GetOrigins(out Point3d[] points);
            if (vectors.Length != 1)
            {
                throw new Exception($"Service \"{hole.Name}\" dont contain vectors or more then 1");
            }
            return new VertexCommand(partName, op, points[0].X);
        }
        private VertexCommand CreateNotchCommand(Line[] lines, string partName)
        {
            // Find midpoint on axis X in line where start and end points is not equal

            var midpoint = lines.Where(x=>x.StartPoint.X != x.EndPoint.X).Select(x => (x.StartPoint.X + x.EndPoint.X) / 2).Max();

            return new VertexCommand(partName, Operations.Notch, midpoint);
        }
        private VertexCommand CreateLipCommand(Line[] lines, string partName)
        {
            var midpoint = lines.Where(x=>x.GetLength() == lines.Max(y=>y.GetLength())).Select(x => (x.StartPoint.X + x.EndPoint.X) / 2).Max();
            // Find midpoint on axis X in line where start and end points is not equal
            MessageBox.Show($"Lip {midpoint}");
            return new VertexCommand(partName, Operations.Lip, midpoint);
        }
        // This commands adds on start and end of profile
        private List<VertexCommand> CreateChamferCommands(double width, string partName)
        {
            List<VertexCommand> commands = new List<VertexCommand>();
            commands.Add(new VertexCommand(partName, Operations.Chamfer, 0));
            commands.Add(new VertexCommand(partName, Operations.Chamfer, width));
            commands.Add(new VertexCommand(partName, Operations.Swage, 33));
            commands.Add(new VertexCommand(partName, Operations.Swage, width - 33));
            return commands;
        }
        private Operations UserAttributeControlBy(Feature feature)
        {
            var attr = feature.GetUserAttributeAsString("Type", NXObject.AttributeType.String, -1);
            switch (attr)
            {
                case "Chamfer":
                    return Operations.Chamfer;
                case "Notch":
                    return Operations.Notch;
                case "Lip":
                    return Operations.Lip;
                default:
                    return Operations.None;
            }
        }
        private T CastNxObjectTo<T>(NXObject obj) where T : NXObject
        {
            try
            {
                return (T)obj;
            }
            catch (Exception)
            {

                throw new Exception($"Cant cast \"{obj.Name}\" with type {obj.GetType().Name} to {typeof(T).Name}");
            }
        }
        private NXObject[] GetGeometryFrom(Feature feature, int exceptedAmount = 0)
        {
            var parents = feature.GetParents();
            if (parents.Length != 1)
            {
                throw new Exception($"feature \"{feature.Name}\" dont contain sketch or contain more then 1");
            }
            var geoms = ((SketchFeature)parents[0]).Sketch.GetAllGeometry();
            if (exceptedAmount == 0)
            {
                if (geoms.Length < 1) throw new Exception($"sketch of feature \"{feature.Name}\" dont contain elements");
                else return geoms;
            }
            else
            {
                if (geoms.Length != exceptedAmount) throw new Exception($"sketch of feature \"{feature.Name}\" containt elements is not equal {exceptedAmount}");
                else return geoms;
            }
        }
        private List<Feature> FindFeaturesBy<T>(BasePart part) where T : class
        {
            try
            {
                return part.Features.ToArray().Where(x => x.GetType().Name == typeof(T).Name).ToList();
            }
            catch (Exception)
            {

                throw new Exception($"part \"{part.Name}\" dont contain feature by type {typeof(T).Name}");
            }
           
        }
        private List<Feature> FindFeaturesBy<T>(BasePart part, string name) where T : class
        {
            try
            {
                return part.Features.ToArray().Where(x => x.GetType().Name == typeof(T).Name && x.FeatureType == name).ToList();

            }
            catch (Exception)
            {

                throw new Exception($"part \"{part.Name}\" dont contain feature by type {typeof(T).Name}. Control name - {name}");
            }
           
        }
        // TODO: ADD SAFETY COMPONENTS
        private BaseAttributes GeteSizeAttributeFrom(ContourFlange flange)
        {
            BaseAttributes Attributes;
            var lines = GetGeometryFrom(flange).Select(x => CastNxObjectTo<Line>(x)).ToArray();
            Line[] topLines;

            topLines =  lines.Where(x => Math.Round(x.StartPoint.Z,3) != Math.Round(x.EndPoint.Z,3) && Math.Round(x.StartPoint.Y,3) == Math.Round(x.EndPoint.Y,3)).ToArray();
            
            Attributes.Hight = topLines.Max(x => x.GetLength());
            
            foreach (var item in flange.GetExpressions())
            {
            }
            Attributes.Width = flange.GetExpressions().Where(x => x.Description.Contains("Width")).First().Value;
            topLines = lines.Where(x => Math.Round(x.StartPoint.Y,3) != Math.Round(x.EndPoint.Y,3) && Math.Round(x.StartPoint.Z,3) == Math.Round(x.EndPoint.Z,3)).ToArray();
            Attributes.Length = topLines.Max(x => x.GetLength());
            Attributes.Thickness = flange.GetExpressions().Where(x => x.Description.Contains("Thickness")).First().Value;
            return Attributes;
        }
        struct BaseAttributes
        {
            internal double Width;
            internal double Hight;
            internal double Length;
            internal double Thickness;
        }
        private double GetDiameterFrom(HolePackage hole)
        {
            try
            {
                return hole.GetExpressions().Where(x => x.Description.Contains("Diameter")).First().Value;
            }
            catch (Exception)
            {

                throw new Exception($"Diameter expression in {hole.Name} dosent exist");
            }
           
        }
    }
}
