using Godot;
using OctoprintClient;
public class Prusacombination : Godot.Spatial
{
    //private GD.OctoprintConnection octoprint;
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    private readonly OctoprintConnection octoprint = new OctoprintConnection("http://192.168.56.102/", "B162D96C9502434F9C28BADB7895D0CD");
    //this is my Virtualbox's local API Key and IP for Octoprint it can't be used or abused by anyone that i know so far, that's why i'm sharing it :D
    private Spatial xAxis = new Spatial();
    private Spatial yAxis = new Spatial();
    private Spatial zAxis = new Spatial();
    private Control printerData = new Control();

    public override void _Ready()
    {
    
    	xAxis = GetNode("Mk3Z/Mk3X") as Spatial;
        yAxis = GetNode("Mk3Y") as Spatial;
        zAxis = GetNode("Mk3Z") as Spatial;
        printerData = GetParent().GetNode("Viewport/PrinterData") as Control;
        octoprint.Position.StartThread();
        octoprint.Position.GetCurrentPosSync();
    }

    public override void _Process(float delta)
    {
        float[] currentPosition = octoprint.Position.GetPosAsync();
        xAxis.Translation = new Vector3(50 + currentPosition[0], 0, 0);
        yAxis.Translation = new Vector3(0, (float)11.25, currentPosition[1]);
        zAxis.Translation = new Vector3(-50, (float)11.25 + currentPosition[2], 0);

    }
    public void _on_Timer_timeout()
    {
        OctoprintFullPrinterState state = octoprint.Printers.GetFullPrinterState();
        (printerData.GetNode("Panel/Flags/Ready") as Label).Visible = state.PrinterState.Flags.Ready;
        (printerData.GetNode("Panel/Flags/Paused") as Label).Visible = state.PrinterState.Flags.Paused;
        (printerData.GetNode("Panel/Flags/Error") as Label).Visible = state.PrinterState.Flags.Error;
        (printerData.GetNode("Panel/Flags/Operational") as Label).Visible = state.PrinterState.Flags.Operational;
        (printerData.GetNode("Panel/Flags/Cancelling") as Label).Visible = state.PrinterState.Flags.Cancelling;
        (printerData.GetNode("Panel/Flags/ClosedOrError") as Label).Visible = state.PrinterState.Flags.ClosedOrError;
        (printerData.GetNode("Panel/Flags/Pausing") as Label).Visible = state.PrinterState.Flags.Pausing;
        (printerData.GetNode("Panel/Flags/Printing") as Label).Visible = state.PrinterState.Flags.Printing;
        (printerData.GetNode("Panel/Flags/SDReady") as Label).Visible = state.PrinterState.Flags.SDReady;
        (printerData.GetNode("Panel/Temps/BedTemperature") as Label).Text = "BedTemperature:\n" + state.TempState.Bed.Actual;
        (printerData.GetNode("Panel/Temps/BedTemperatureTarget") as Label).Text = "Target:\n" + state.TempState.Bed.Target;
        (printerData.GetNode("Panel/Temps/BedTemperatureOffset") as Label).Text = "Offset:\n" + state.TempState.Bed.Offset;
        (printerData.GetNode("Panel/Temps/ToolTemperature") as Label).Text = "ToolTemperature:\n" + state.TempState.Tools[0].Actual;
        (printerData.GetNode("Panel/Temps/ToolTemperatureTarget") as Label).Text = "Target:\n" + state.TempState.Tools[0].Target;
        (printerData.GetNode("Panel/Temps/ToolTemperatureOffset") as Label).Text = "Offset:\n" + state.TempState.Tools[0].Offset;
        (printerData.GetNode("Panel/Heading") as Label).Text="State: " + state.PrinterState.Text;
        OctoprintJobProgress progress=octoprint.Jobs.GetProgress();
        (printerData.GetNode("Panel/ProgressBar/TimeT") as Label).Text = "Time: " + progress.PrintTime / 60 + ":" + progress.PrintTime % 60;
        (printerData.GetNode("Panel/ProgressBar/TimeL") as Label).Text = "Time: " + progress.PrintTimeLeft / 60 + ":" + progress.PrintTimeLeft % 60;
        (printerData.GetNode("Panel/ProgressBar") as ProgressBar).Value = (float)progress.Completion;
        GD.Print("progress: " + (float)progress.Completion);

    }
}

