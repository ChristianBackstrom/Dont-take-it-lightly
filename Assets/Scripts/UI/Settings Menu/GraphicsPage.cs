using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsPage : SettingsPage
{

    GpuMode currentMode;
    GpuMode nextMode;
    public ScreenSize[] modes;
    public FullScreenMode[] fsModes = {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };
    int modeIndex, fsModeIndex;

    [SerializeField] TextMeshProUGUI resolution, mode;





    void OnEnable()
    {
        currentMode = GetMode();
        nextMode = GetMode();
        modes = GetModes();
        
        for (int i = 0; i < modes.Length; i++)
        {
            if (modes[i].width == currentMode.screenSize.width && modes[i].height == currentMode.screenSize.height)
            {
                modeIndex = i;
            }
        }

        switch (currentMode.fullScreenMode)
        {
            case (FullScreenMode.ExclusiveFullScreen):
            {
                fsModeIndex = 0;
                break;
            }
            case (FullScreenMode.FullScreenWindow):
            {
                fsModeIndex = 1;
                break;
            }
            case (FullScreenMode.Windowed):
            {
                fsModeIndex = 2;
                break;
            }
        }

        UpdateText();

    }

    public void nextResolution()
    {
        modeIndex++;
        if (modeIndex == modes.Length) modeIndex = 0;

        UpdateText();
    }

    public void nextFsMode()
    {
        fsModeIndex++;
        if (fsModeIndex == fsModes.Length) fsModeIndex = 0;

        UpdateText();
    }

    public void prevResolution()
    {
        modeIndex--;
        if (modeIndex == -1) modeIndex = modes.Length - 1;

        UpdateText();
    }

    public void prevFsMode()
    {
        fsModeIndex--;
        if (fsModeIndex == -1) fsModeIndex = fsModes.Length - 1;

        UpdateText();
    }




    void UpdateText()
    {
        resolution.SetText(" " + modes[modeIndex].width + " x " + modes[modeIndex].height);
        switch (fsModes[fsModeIndex])
        {
            case FullScreenMode.ExclusiveFullScreen:
            {
                mode.SetText("Fullscreen");
                break;
            }
            case FullScreenMode.FullScreenWindow:
            {
                mode.SetText("Windowed Fullscreen");
                break;
            }
            case FullScreenMode.Windowed:
            {
                mode.SetText("Windowed");
                break;
            }
        }

        ApplyToTemp();

    }

    void ApplyToTemp()
    {
        nextMode.screenSize = modes[modeIndex];
        nextMode.fullScreenMode = fsModes[fsModeIndex];
    }

    public void ApplyTempSetting()
    {
        ApplySetting(nextMode);
    }

    public void RevertToOldSetting()
    {
        ApplySetting(currentMode);
    }

    void ApplySetting(GpuMode gpuMode)
    {
        Screen.SetResolution(gpuMode.screenSize.width, gpuMode.screenSize.height, gpuMode.fullScreenMode);
    }


    GpuMode GetMode()
    {
        return new GpuMode(Screen.height, Screen.width, Screen.fullScreenMode);
    }

    ScreenSize[] GetModes()
    {
        var input = Screen.resolutions;
        List<ScreenSize> output = new List<ScreenSize>();

        for (int i = 0; i < input.Length; i++)
        {
            if (i > 0)
            {
                if (input[i].width == input[i - 1].width && input[i].height == input[i - 1].height) continue;
            }
            /*todo: sort out weird aspect ratios */
            /*if (width / height == weird) continue; */
            output.Add(new ScreenSize(input[i].height, input[i].width));
        }

        output.Add(new ScreenSize(Screen.height, Screen.width));

        return output.ToArray();
    }

    public class GpuMode
    {
        public ScreenSize screenSize;
        public FullScreenMode fullScreenMode;

        public GpuMode(int height, int width, FullScreenMode fullScreenMode)
        {
            this.screenSize = new ScreenSize(height, width);
            this.fullScreenMode = fullScreenMode;
        }

        public GpuMode(GpuMode oldMode)
        {
            this.screenSize = oldMode.screenSize;
            this.fullScreenMode = oldMode.fullScreenMode;
        }
    }

    public class ScreenSize
    {
        public int height, width;

        public ScreenSize(int height, int width)
        {
            this.height = height;
            this.width = width;
        }
    }




    
}




