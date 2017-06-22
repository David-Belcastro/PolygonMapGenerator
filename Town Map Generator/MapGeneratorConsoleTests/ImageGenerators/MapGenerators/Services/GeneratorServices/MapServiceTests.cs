using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrainGenerator.Services.GeneratorServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;
using ceometric.DelaunayTriangulator;
using BrnVoronoi.Models;

namespace TerrainGenerator.Services.GeneratorServices.Tests
{
    [TestClass()]
    public class MapServiceTests
    {

        [TestMethod()]
        public void DotInMap_DotIn_returnsTrue()
        {
            var mapService = new MapService(MockRepository.GenerateMock<MapGenService>(), 10, 10, 10, MockRepository.GenerateMock<ILandFormGenerator>());
            Assert.IsTrue(mapService.DotInMap(1, 1));
        }
                


        [TestMethod()]
        public void DotInMap_DotOut_returnsFalse() {
            var mapService = new MapService(MockRepository.GenerateMock<MapGenService>(), 10, 10, 10, MockRepository.GenerateMock<ILandFormGenerator>());
            Assert.IsFalse(mapService.DotInMap(15, -1));
        }


        [TestMethod()]
        public void DotInMap_DotHalfInX_returnsFalse()
        {
            var mapService = new MapService(MockRepository.GenerateMock<MapGenService>(), 10, 10, 10, MockRepository.GenerateMock<ILandFormGenerator>());
            Assert.IsFalse(mapService.DotInMap(3, -1));
        }


        [TestMethod()]
        public void DotInMap_DotHalfInY_returnsFalse()
        {
            var mapService = new MapService(MockRepository.GenerateMock<MapGenService>(), 10, 10, 10, MockRepository.GenerateMock<ILandFormGenerator>());
            Assert.IsFalse(mapService.DotInMap(15, 5));
        }
    }
}