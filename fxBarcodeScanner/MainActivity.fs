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
open Android.Gms.Vision

[<Activity (Label = "fxBarcodeScanner", MainLauncher = true)>]
type MainActivity () =
    inherit Activity ()

    override this.OnCreate (bundle) =
        base.OnCreate (bundle)
        this.SetContentView (Resource_Layout.Main)

        let appContext = Application.Context
        let imageView = this.FindViewById<ImageView>(Resource_Id.ImageView)
        let textView = this.FindViewById<TextView>(Resource_Id.TextView) 

        let bitmap = BitmapFactory.DecodeResource(appContext.Resources, Resource_Drawable.Puppy)
        imageView.SetImageBitmap(bitmap)

        let detector = (new BarcodeDetector.Builder(appContext))
                                               .SetBarcodeFormats(BarcodeFormat.QrCode)
                                               .Build()

        let button = this.FindViewById<Button>(Resource_Id.MyButton)
        button.Click.Add (fun args -> 
            if not detector.IsOperational then
                textView.Text <- "Not operational"
            else
                textView.Text <- "Operational"

                let frame = (new Frame.Builder()).SetBitmap(bitmap).Build()
                let barcode = detector.Detect(frame).ValueAt(0) :?> Barcode 

                textView.Text <- barcode.RawValue
        )
