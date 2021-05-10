using System;
using Xunit;

namespace GoProFixer.Test
{
    public class GoProFileRenameTest
    {
        private readonly GoProFileRename _target;

        public GoProFileRenameTest()
        {
            _target = new GoProFileRename();
        }

        [Theory]
        [InlineData("GH010120.THM")]
        [InlineData("GH010120.MP4")]
        [InlineData("GH010120.LRV")]
        [InlineData("GL010120.THM")]
        [InlineData("GL010120.MP4")]
        [InlineData("GL010120.LRV")]
        public void IsAMatch(string filename)
        {
            var shouldBeRenamed = _target.ShouldBeRenamed("c:\\test\\" + filename);
            
            Assert.True(shouldBeRenamed);
        }

        [Theory]
        [InlineData("FH010120.THM")]
        [InlineData("GH010120.MP3")]
        [InlineData("GH0010120.LRV")]
        [InlineData("GL0101220.THM")]
        public void IsNoMatch(string filename)
        {
            var shouldBeRenamed = _target.ShouldBeRenamed("c:\\test\\" + filename);

            Assert.False(shouldBeRenamed);
        }
        
        [Theory]
        [InlineData(@"c:\test\GH010120.MP4", @"c:\test\GH0120_01.MP4")]
        [InlineData(@"c:\test\GL010120.MP4", @"c:\test\GL0120_01.MP4")]
        [InlineData(@"c:\test\GH010120.LRV", @"c:\test\GH0120_01.LRV")]
        public void IsRenamedTo(string fullFilenameInput, string fullFilenameExpected)
        {
            var result = _target.GetFileRenameInfo(fullFilenameInput);

            Assert.Equal(fullFilenameExpected, result.NewName);
        }
    }
}
