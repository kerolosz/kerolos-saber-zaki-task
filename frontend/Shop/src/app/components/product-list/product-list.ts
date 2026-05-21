import { Component, OnInit, signal } from '@angular/core';
import { Productservice } from '../../services/productservice';
import { Product } from '../../models/Product';
import { ApiResponse } from '../../models/Response';
declare var bootstrap: any;

@Component({
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList implements OnInit {
  products = signal<ApiResponse<Product[]>>({} as ApiResponse<Product[]>);
  constructor(private _Productservice: Productservice) {}

  ngOnInit(): void {
    this.getProducts();
  }

  getProducts() {
    this._Productservice.getProducts().subscribe((response) => {
      this.products.set(response);
    });
  }

  //Implement delete functionality
  selectedProduct: any = null;

  openDeleteModal(product: any) {
    this.selectedProduct = product;

    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));

    modal.show();
  }

  confirmDelete() {
    this._Productservice.deleteProduct(this.selectedProduct.id).subscribe({
      next: () => {
        this.getProducts();
      },
      error: (error) => {
        console.error('Error deleting product:', error);
      },
    });

    this.selectedProduct = null;

    const modalEl = document.getElementById('deleteModal');
    const modal = bootstrap.Modal.getInstance(modalEl);
    modal?.hide();
  }
  ///////////////////////
}
