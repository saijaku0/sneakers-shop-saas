import { capitalize } from "@/shared/lib";
import { NavItem } from "../model/types";

type ApiData = {
  categories: { men: string[]; women: string[]; kids: string[] };
  brands: { name: string; slug: string }[];
};

export const buildNavigation = (apiData: ApiData): NavItem[] => {
  const genderItems = Object.entries(apiData.categories).map(
    ([gender, categories]) => {
      const normalizedGender = capitalize(gender);

      return {
        label: normalizedGender,
        path: `/${gender}`,
        filter: {
          gender: normalizedGender,
        },
        sections: [
          {
            title: "Footwear",
            links: categories.map((type) => ({
              label: type,
              path: `/${gender}-${type.toLowerCase()}`,
              filter: {
                gender: normalizedGender,
                type,
              },
            })),
          },
        ],
      };
    },
  );

  return [
    ...genderItems,

    {
      label: "Brands",
      path: "/brands",
      sections: [
        {
          title: "Popular Brands",
          links: apiData.brands.map((brand) => ({
            label: brand.name,
            path: `/brands/${brand.slug}`,
            filter: {
              brand: brand.name,
            },
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
