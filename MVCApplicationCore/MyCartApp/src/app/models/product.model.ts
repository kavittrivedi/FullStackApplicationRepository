import { ProductCategory } from "./product.category.model";

export interface Product{
    productId: number;
    productName: string;
    productDescription: string;
    categoryId: number;
    productPrice: number;
    category: ProductCategory;
    inStock: boolean;
    isActive: boolean;
}