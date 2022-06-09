Source: https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET

Presorting the images by the 'thumbnail' before comparison appears to be flawed.
If the images differ in the first few bytes of the thumbnail, but are the same in the rest of the thumbnail,
then sorting them places them far apart in the array, and so comparing each image to previous will not find them as a match.


