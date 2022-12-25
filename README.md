# Native Review
A quick implementation of Native In-App Review for Unity

# How To Use
1. Import the package to your project.
2. Call "NativeReview.Instance.LoadReview()" to load the in-app review beforehand.
3. Call "NativeReview.Instance.ShowReview()" to display the native review to users.

# Note
1. You can use the events "OnReviewLoaded" and "OnReviewDisplayed" to know what-is-going-on when calling above methods.
2. The native review will NOT display everytime you call because of AppStore/PlayStore's rules.
- https://developer.android.com/guide/playcore/in-app-review
- https://developer.apple.com/app-store/review/

# Troubleshooting
1. AndroidJavaException: java.lang.ClassNotFoundException: com.google.android.play.core.review
- Disable proguard function in PlayerSettings > Minify > Release/Debug

------
Happy Developing!
