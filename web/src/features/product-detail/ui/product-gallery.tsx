/* eslint-disable @next/next/no-img-element */
"use client";

export function ProductGallery({
  images,
  alt,
}: {
  images: string[];
  alt: string;
}) {
  if (images.length === 0) {
    return (
      <div className="flex aspect-square items-center justify-center bg-muted text-muted-foreground">
        No image
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
      {images.map((img, i) => (
        <div
          key={i}
          className={`aspect-square overflow-hidden bg-muted ${
            // первое фото на всю ширину, если картинок нечётно/мало — красивее
            images.length === 1 ? "sm:col-span-2" : ""
          }`}
        >
          <img
            src={img}
            alt={`${alt} ${i + 1}`}
            className="h-full w-full object-cover object-center"
          />
        </div>
      ))}
    </div>
  );
}
