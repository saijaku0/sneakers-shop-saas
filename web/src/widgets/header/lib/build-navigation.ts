import { NavItem } from "../model/types";

type ApiData = {
  categories: { men: string[]; women: string[]; kids: string[] };
  brands: { name: string; slug: string }[];
};

export const buildNavigation = (apiData: ApiData): NavItem[] => {
  return [
    {
      label: "Men",
      path: "/men",
      filter: { gender: "Men" },
      sections: [
        {
          title: "Footwear",
          links: apiData.categories.men.map((type) => ({
            label: type,
            path: `/men/${type.toLowerCase()}`,
            filter: { gender: "Men", type: type },
          })),
        },
      ],
    },
    {
      label: "Women",
      path: "/women",
      filter: { gender: "Women" },
      sections: [
        {
          title: "Footwear",
          links: apiData.categories.women.map((type) => ({
            label: type,
            path: `/women/${type.toLowerCase()}`,
            filter: { gender: "Women", type: type },
          })),
        },
      ],
    },
    {
      label: "Kids",
      path: "/kids",
      filter: { gender: "Kids" },
      sections: [
        {
          title: "Footwear",
          links: apiData.categories.kids.map((type) => ({
            label: type,
            path: `/kids/${type.toLowerCase()}`,
            filter: { gender: "Kids", type: type },
          })),
        },
      ],
    },
    {
      label: "Brands",
      path: "/catalog",
      sections: [
        {
          title: "Popular Brands",
          links: apiData.brands.map((brand) => ({
            label: brand.name,
            path: `/brands/${brand.slug}`,
            filter: { brand: brand.name },
          })),
        },
      ],
    },
    {
      label: "Sale",
      path: "/sale",
    },
  ];
};
