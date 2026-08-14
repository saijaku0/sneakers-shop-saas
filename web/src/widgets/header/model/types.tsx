export interface NavLink {
  label: string;
  path: string;
  filter?: {
    gender?: string;
    type?: string;
    brand?: string;
  };
}

export interface NavSection {
  title: string;
  links: NavLink[];
}

export interface NavItem {
  label: string;
  path: string;
  filter?: {
    gender?: string;
  };
  sections?: NavSection[];
}
