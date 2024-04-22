using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using PAB.Objects;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PAB.Model
{
    public class PresnetationObject
    {
        Slides slides;
        _Slide slide;
        TextRange objText;

        Microsoft.Office.Interop.PowerPoint.Application pptApplication;
        Presentation pptPresentation;
        readonly CustomLayout customLayout;
        readonly float convertToCM = 0.035f;

        int musicFontSize;
        int bibleFontSize;
        string fontName;
        string bakcgoundFilePath;

        public PresnetationObject()
        {
            pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();
            pptPresentation = pptApplication.Presentations.Add(MsoTriState.msoTrue);
            customLayout = pptPresentation.SlideMaster.CustomLayouts[PpSlideLayout.ppLayoutChartAndText];
        }

        public void CreatePowerPointSlides(Setting setting)
        {
            musicFontSize = int.Parse(setting.musicFontSize);
            bibleFontSize = int.Parse(setting.bibleFontSize);
            fontName = setting.fontName;
            bakcgoundFilePath = setting.backgoundFilePath;
            
            ObservableCollection <ShowingObject> list = ListManager.GetMakeList();
            foreach (ShowingObject showingObject in list.Reverse())
            {
                slide = MakeSlide("", false);

                string content = showingObject.content;
                string[] contents = content.Split(new string[] { "\n\n" }, StringSplitOptions.None);
                for (int i = 0; i < contents.Length; i++)
                {
                    slide = MakeSlide(contents[(contents.Length - 1) - i], showingObject is Bible);
                }
            }
        }

        private _Slide MakeMusicSlide(string lyrics)
        {
            slides = pptPresentation.Slides;
            slide = slides.AddSlide(1, customLayout);
            slide.BackgroundStyle = MsoBackgroundStyleIndex.msoBackgroundStylePreset4;

            slide.Shapes[1].Width = 32 / convertToCM;
            slide.Shapes[1].Height = 18 / convertToCM;
            slide.Shapes[1].Top = 20;
            slide.Shapes[1].Left = 0;

            slide.Shapes[1].TextFrame.VerticalAnchor = MsoVerticalAnchor.msoAnchorTop;

            objText = slide.Shapes[1].TextFrame.TextRange;
            objText.Text = lyrics;
            objText.ParagraphFormat.Alignment = PpParagraphAlignment.ppAlignCenter;
            objText.Font.NameFarEast = fontName;
            objText.Font.Name = fontName;
            objText.Font.Size = musicFontSize;
            objText.Font.Bold = MsoTriState.msoTrue;
            objText.Font.Color.RGB = 16777215;

            if (bakcgoundFilePath != "파일 선택")
            {
                try
                {
                    slide.Shapes.AddPicture2(bakcgoundFilePath, MsoTriState.msoFalse, MsoTriState.msoCTrue, 0, 0, 32 * 30, 18 * 30);
                    slide.Shapes[2].ZOrder(MsoZOrderCmd.msoSendBackward);
                }
                catch
                {
                    MessageBox.Show("배경 파일 경로 오류");
                }
            }
            return slide;
        }

        private _Slide MakeBibleSlide(string lyrics)
        {
            slides = pptPresentation.Slides;
            slide = slides.AddSlide(1, customLayout);
            slide.BackgroundStyle = MsoBackgroundStyleIndex.msoBackgroundStylePreset4;

            slide.Shapes[1].Width = 15 / convertToCM;
            slide.Shapes[1].Height = 2 / convertToCM;
            slide.Shapes[1].Top = 4.9f / convertToCM;
            slide.Shapes[1].Left = 2.6f / convertToCM;

            //slide.Shapes[1].TextFrame.VerticalAnchor = MsoVerticalAnchor.msoAnchorTop;

            var lyricsList = lyrics.Split('\n');
            objText = slide.Shapes[1].TextFrame.TextRange;
            objText.Text = lyricsList[0];
            objText.Font.NameFarEast = fontName;
            objText.Font.Name = fontName;
            objText.Font.Size = bibleFontSize;
            objText.Font.Color.RGB = 16777215;

            var shapes = slide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 2.6f / convertToCM, 8 / convertToCM, 30 / convertToCM, 8 / convertToCM);
            objText = shapes.TextFrame.TextRange;
            string content = "";
            for (int index = 1; index < lyricsList.Length; index++)
            {
                content += lyricsList[index] + "\n";
            }
            objText.Text = content;
            objText.Font.NameFarEast = fontName;
            objText.Font.Name = fontName;
            objText.Font.Size = bibleFontSize;
            objText.Font.Bold = MsoTriState.msoTrue;
            objText.Font.Color.RGB = 16777215;

            Microsoft.Office.Interop.PowerPoint.Shape[] shapesList = new Microsoft.Office.Interop.PowerPoint.Shape[3];
            for (int index = 0; index < 3; index++)
            {
                var circle = slide.Shapes.AddShape(MsoAutoShapeType.msoShapeOval, 3 / convertToCM, (3 + index * 0.45f) / convertToCM, 0.3f / convertToCM, 0.3f / convertToCM);
                circle.Fill.Visible = MsoTriState.msoFalse;
                circle.Line.ForeColor.RGB = (255 << 16) | (255 <<  8) | 255;
                circle.Line.Weight = 1.0f; // 두께를 1.0포인트로 설정

                shapesList[index] = circle;
            }

            var lineShape = slide.Shapes.AddLine(31 / convertToCM, 17.4f / convertToCM, 31 / convertToCM, 17.4f / convertToCM);
            lineShape.Width = 2 / convertToCM;
            lineShape.Line.ForeColor.RGB = (255 << 16) | (255 << 8) | 255;
            lineShape.Line.Weight = 0.5f;
                
            if (bakcgoundFilePath != "파일 선택")
            {
                try
                {
                    slide.Shapes.AddPicture2(bakcgoundFilePath, MsoTriState.msoFalse, MsoTriState.msoCTrue, 0, 0, 32 * 30, 18 * 30);
                    slide.Shapes[2].ZOrder(MsoZOrderCmd.msoSendBackward);
                }
                catch
                {
                    MessageBox.Show("배경 파일 경로 오류");
                }
            }
            return slide;
        }

        private _Slide MakeSlide(string lyrics, bool isBible)
        {
            if (isBible)
            {
                return MakeBibleSlide(lyrics);
            }
            else
            {
                return MakeMusicSlide(lyrics);
            }
        }
    }
}
