# Public Images

This folder contains **static images** that are served as-is without processing or optimization.

## Usage

### In Astro components:

```astro
<img src="/images/favicon.png" alt="Favicon" />
```

### In Markdown:

```markdown
![Logo](/images/logo.png)
```

### With Astro's Image component (requires width & height):

```astro
---
import { Image } from 'astro:assets';
---

<Image 
  src="/images/banner.jpg" 
  alt="Banner" 
  width="1200" 
  height="400" 
/>
```

## When to Use

Use this folder for:

- **Favicons and manifest icons**
- **Third-party assets** that shouldn't be modified
- **Large files** that don't need optimization
- **Assets referenced by name** in external systems
- **SEO images** (robots.txt, sitemap images)

## Important Notes

- Files are copied to build output **without transformation**
- Images are **NOT optimized** automatically
- Use absolute paths starting with `/images/...`
- No TypeScript type checking for these assets

## Organization

```
public/images/
├── favicons/
├── social/       # Open Graph, Twitter cards
├── seo/          # Schema.org images
└── third-party/  # External logos, badges
```
