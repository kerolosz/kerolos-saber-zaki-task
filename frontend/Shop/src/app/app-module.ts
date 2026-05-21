import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { ProductList } from './components/product-list/product-list';
import { CommonModule } from '@angular/common';
import { CreateProduct } from './components/create-product/create-product';
import { ReactiveFormsModule } from '@angular/forms';
import { UpdateProduct } from './components/update-product/update-product';
import { NgxSpinnerModule } from 'ngx-spinner';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { spinnerInterceptor } from './interceptors/spinnerInterceptor';

@NgModule({
  declarations: [App, ProductList, CreateProduct, UpdateProduct],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CommonModule,
    ReactiveFormsModule,
    NgxSpinnerModule.forRoot({}),
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([spinnerInterceptor])),
  ],
  bootstrap: [App],
})
export class AppModule {}
