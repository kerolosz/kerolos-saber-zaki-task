export interface Product {
  id: number;
  name: string;
  description: string;
  oldPrice: number;
  newPrice: number;
  imagePath: string;
  createdBy: string;
  createdDate: string;
  updatedBy: string | null;
  updatedDate: string | null;
}
