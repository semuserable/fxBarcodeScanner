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

    [<Literal>]
    let REQUEST_CODE = 1

    let mutable _imageView: Option<ImageView> = None
    let mutable _textView: Option<TextView> = None
    let mutable _button: Option<Button> = None

    let mutable _detector: Option<BarcodeDetector> = None

    let testBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource_Drawable.Puppy)

    let getBarcodeRawValue (detector: BarcodeDetector) bitmap = 
        let frame = (new Frame.Builder()).SetBitmap(bitmap).Build()
        let detected = detector.Detect(frame)
        match detected.Size() with
        | 0 -> "Invalid barcode!"
        | _ -> (detector.Detect(frame).ValueAt(0) :?> Barcode).RawValue

    member this.OperationalAction (dataIntent: Intent) =
        let stream = this.ContentResolver.OpenInputStream(dataIntent.Data)
        let bitmap = BitmapFactory.DecodeStream(stream);

        _imageView |> Option.iter (fun i -> i.SetImageBitmap(bitmap))

        let raw = getBarcodeRawValue _detector.Value bitmap
        _textView |> Option.iter (fun t -> t.Text <- sprintf "From activity: %A" raw)

    member this.CameraIntent =
        let intent = (new Intent()).SetType("image/*").SetAction(Intent.ActionGetContent).AddCategory(Intent.CategoryOpenable);
        this.StartActivityForResult(intent, REQUEST_CODE);

    override this.OnCreate (bundle) =
        base.OnCreate (bundle)
        this.SetContentView (Resource_Layout.Main)

        _imageView <- this.FindViewById<ImageView>(Resource_Id.ImageView) |> Some
        _textView <- this.FindViewById<TextView>(Resource_Id.TextView) |> Some
        _button <- this.FindViewById<Button>(Resource_Id.MyButton) |> Some

        _imageView |> Option.iter (fun b -> b.SetImageBitmap(testBitmap))

        let supportedBarcodes = BarcodeFormat.QrCode ||| BarcodeFormat.UpcA ||| BarcodeFormat.UpcE
        _detector <- (new BarcodeDetector.Builder(Application.Context))
                                               .SetBarcodeFormats(supportedBarcodes)
                                               .Build() |> Some
        let detector = _detector.Value

        _button |> Option.iter (fun b -> b.Click.Add (fun args -> 
            if not detector.IsOperational then
                _textView |> Option.iter (fun t -> t.Text <- "Not operational")
            else
                _textView |> Option.iter (fun t -> t.Text <- getBarcodeRawValue detector testBitmap; this.CameraIntent)
        ))
    
    override this.OnActivityResult (requestCode: int, resultCode: Result, data: Intent) =
        match (requestCode, resultCode) with
        | (REQUEST_CODE, Result.Ok) -> this.OperationalAction data
        | _ -> ()