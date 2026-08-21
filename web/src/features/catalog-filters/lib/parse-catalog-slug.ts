const capitalize = (s: string) =>
  s ? s.charAt(0).toUpperCase() + s.slice(1).toLowerCase() : "";

export function parseCatalogSlug(slug: string): {
  gender: string;
  category: string;
} {
  const dash = slug.indexOf("-");
  if (dash === -1) return { gender: capitalize(slug), category: "" };

  return {
    gender: capitalize(slug.slice(0, dash)),
    category: capitalize(slug.slice(dash + 1)),
  };
}
