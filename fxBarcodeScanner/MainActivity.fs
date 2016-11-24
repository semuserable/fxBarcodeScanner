namespace fxBarcodeScanner

open System

open Android.App
open Android.Content
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget
open Android.Graphics
open Android.Gms.Vision.Barcodes

[<Activity (Label = "fxBarcodeScanner", MainLauncher = true)>]
type MainActivity () =
    inherit Activity ()

    override this.OnCreate (bundle) =
        base.OnCreate (bundle)
        this.SetContentView (Resource_Layout.Main)

        let appContext = Application.Context
        let imageView = this.FindViewById<ImageView>(Resource_Id.ImageView)
        let textView = this.FindViewById<TextView>(Resource_Id.TextView) 

        BitmapFactory.DecodeResource(appContext.Resources, Resource_Drawable.Puppy)
        |> imageView.SetImageBitmap

        let detector = (new BarcodeDetector.Builder(appContext))
                                               .SetBarcodeFormats(BarcodeFormat.QrCode)
                                               .Build()

        let button = this.FindViewById<Button>(Resource_Id.MyButton)
        button.Click.Add (fun args -> 
            if not detector.IsOperational then
                textView.Text <- "Not operational"
            else
                textView.Text <- "Operational"
        )
