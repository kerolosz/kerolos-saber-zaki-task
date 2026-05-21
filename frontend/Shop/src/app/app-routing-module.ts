import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductList } from './components/product-list/product-list';
import { CreateProduct } from './components/create-product/create-product';
import { UpdateProduct } from './components/update-product/update-product';

const routes: Routes = [
  { path: '', component: ProductList, pathMatch: 'full' },
  { path: 'addProduct', component: CreateProduct },
  { path: 'updateProduct/:id', component: UpdateProduct },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
