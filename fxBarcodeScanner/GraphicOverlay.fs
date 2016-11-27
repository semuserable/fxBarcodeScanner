module BarcodeTrackerFactory

open Android.Content
open Android.Graphics
open Android.Util
open Android.Views
open Android.Gms.Vision
open Android.Hardware
open Android.Util

type CameraSource() =
    static member CAMERA_FACING_FRONT = CameraFacing.Front
    static member CAMERA_FACING_BACK = CameraFacing.Back

module GraphicOverlay =
    let WIDTH_SCALE_FACTOR = 1.0f
    let HEIGHT_SCALE_FACTOR = 1.0f
    let FACING = CameraSource.CAMERA_FACING_BACK

    type Graphic() = // implement comparable
        abstract member Draw: Canvas -> unit
        default this.Draw canvas = ()

        member this.ScaleX horizontal = horizontal * WIDTH_SCALE_FACTOR
        member this.ScaleY vertical = vertical * HEIGHT_SCALE_FACTOR
        member this.TranslateX facing overlayWidth x = 
            if facing = CameraSource.CAMERA_FACING_FRONT then 
                overlayWidth - this.ScaleX(x)
            else
                this.ScaleX(x)
        member this.TranslateY y = this.ScaleY(y)
    
    // TODO: https://github.com/googlesamples/android-vision/blob/master/visionSamples/barcode-reader/app/src/main/java/com/google/android/gms/samples/vision/barcodereader/ui/camera/GraphicOverlay.java
    type T<'a when 'a :> Graphic and 'a: comparison>(context: Context, attributeSet: IAttributeSet) = 
        inherit View(context, attributeSet)

        let mutable _graphics: Set<'a> = Set.empty

        member this.GetWidth() = 5.0f // from View subclass

        member this.Add graphic =
            _graphics <- Set.empty.Add graphic
            this.PostInvalidate()

        member this.Remove graphic =
            _graphics <- _graphics.Remove(graphic)
            this.PostInvalidate()

        member this.Clear = 
            _graphics <- Set.empty
            this.PostInvalidate()

        member this.GetGraphics() = Set.toList _graphics

//
//    /**
//     * Sets the camera attributes for size and facing direction, which informs how to transform
//     * image coordinates later.
//     */
//    public void setCameraInfo(int previewWidth, int previewHeight, int facing) {
//        synchronized (mLock) {
//            mPreviewWidth = previewWidth;
//            mPreviewHeight = previewHeight;
//            mFacing = facing;
//        }
//        postInvalidate();
//    }
//
//    /**
//     * Draws the overlay with its associated graphic objects.
//     */
//    @Override
//    protected void onDraw(Canvas canvas) {
//        super.onDraw(canvas);
//
//        synchronized (mLock) {
//            if ((mPreviewWidth != 0) && (mPreviewHeight != 0)) {
//                mWidthScaleFactor = (float) canvas.getWidth() / (float) mPreviewWidth;
//                mHeightScaleFactor = (float) canvas.getHeight() / (float) mPreviewHeight;
//            }
//
//            for (Graphic graphic : mGraphics) {
//                graphic.draw(canvas);
//            }
//        }
//    }