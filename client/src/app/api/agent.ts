import { history } from "./../../index";
import axios, { AxiosError, AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { PaginatedResponse } from "../models/pagination";
import { store } from "../store/configureStore";

const sleep = () => new Promise((resolve) => setTimeout(resolve, 500));

axios.defaults.baseURL = process.env.REACT_APP_API_URL;
axios.defaults.withCredentials = true;

axios.interceptors.request.use((config) => {
  const token = store.getState().account.user?.token;
  if (token) config.headers!.Authorization = `Bearer ${token}`;
  return config;
});

axios.interceptors.response.use(
  async (response) => {
    if (process.env.NODE_ENV === "development") await sleep();
    const pagination = response.headers["pagination"]; // ブラウザで capitalcase でも axios は lowercase の必要
    if (pagination) {
      response.data = new PaginatedResponse(
        response.data,
        JSON.parse(pagination)
      );
      return response;
    }
    return response;
  },
  (error: AxiosError) => {
    const { data, status } = error.response as any; // as any を付けないとエラーになる
    switch (status) {
      case 400:
        if (data.errors) {
          // errors を持つのは ModelState.AddModelError でエラーを追加した "validation-error" のみ
          const modelStateErrors: string[] = [];
          for (const key in data.errors) {
            if (data.errors[key]) {
              // typescript エラーを出さないためのボイラープレート
              modelStateErrors.push(data.errors[key]);
            }
          }
          throw modelStateErrors.flat(); // 2重配列なので flat() で 1 次元化する。throw しているので toast.error まで実行されない
        }
        toast.error(data.title);
        break;
      case 401:
        toast.error(data.title);
        break;
      case 500: // トーストを表示するのではなく、エラーページを表示する
        history.push("server-error", { error: data });
        break;
      default:
        break;
    }
    // エラーの場合も reject しないと then(responseBody) が発火してしまうため、reject する
    // reject 後はコンポーネント側の catch でエラーハンドリングする
    return Promise.reject(error.response); // コンポーネント側で error.response をする必要がなくなる
  }
);

const responseBody = (response: AxiosResponse) => response.data;

const requests = {
  // URLSearchParams は node.js の interface
  get: (url: string, params?: URLSearchParams) =>
    axios.get(url, { params: params }).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
  delete: (url: string) => axios.delete(url).then(responseBody),
};

const Catalog = {
  list: (params: URLSearchParams) => requests.get("products", params),
  details: (id: number) => requests.get(`products/${id}`),
  fetchFilters: () => requests.get("products/filters"),
};

const TestErrors = {
  get400Error: () => requests.get("buggy/bad-request"),
  get401Error: () => requests.get("buggy/unauthorized"),
  get404Error: () => requests.get("buggy/not-found"),
  get500Error: () => requests.get("buggy/server-error"),
  getValidationError: () => requests.get("buggy/validation-error"),
};

const Basket = {
  get: () => requests.get("basket"), // API は Cookie からデータを取るので引数は必要ない
  addItem: (productId: number, quantity = 1) =>
    requests.post(`basket?productId=${productId}&quantity=${quantity}`, {}),
  removeItem: (productId: number, quantity = 1) =>
    requests.delete(`basket?productId=${productId}&quantity=${quantity}`),
};

const Account = {
  login: (values: any) => requests.post("account/login", values),
  register: (values: any) => requests.post("account/register", values),
  currentUser: () => requests.get("account/currentUser"), // ヘッダーで jwt を送るため、パラメータは必要ない
  fetchAddress: () => requests.get("account/savedAddress"),
};

const Orders = {
  list: () => requests.get("orders"),
  fetch: (id: number) => requests.get(`orders/${id}`),
  create: (values: any) => requests.post(`orders`, values),
};

const Payments = {
  createPaymentIntent: () => requests.post("payment", {})
}

const agent = {
  Catalog,
  TestErrors,
  Basket,
  Account,
  Orders,
  Payments
};

export default agent;
