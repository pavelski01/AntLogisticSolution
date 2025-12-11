# Assets Images

This folder contains images that will be **optimized and processed** by Astro's build system.

## Usage

### Import and use with Astro's Image component:

```astro
---
import { Image } from 'astro:assets';
import logo from './images/logo.png';
---

<Image src={logo} alt="Company Logo" />
```

### Import and use in React components:

```tsx
import heroImage from '@/assets/images/hero.png';

export default function Hero() {
  return <img src={heroImage.src} alt="Hero" />;
}
```

## Benefits

- **Automatic optimization**: Images are automatically converted to modern formats (WebP, AVIF)
- **Lazy loading**: Built-in lazy loading for better performance
- **Responsive images**: Automatic generation of multiple sizes
- **Type safety**: Full TypeScript support with ImageMetadata

## Organization

Organize images by feature or page:

```
src/assets/images/
├── logos/
├── heroes/
├── products/
└── icons/
```

## Supported Formats

- PNG
- JPEG/JPG
- GIF
- SVG
- WebP
- AVIF
