import { createContext, PropsWithChildren, useContext, useState } from "react";
import { Basket } from "../models/basket";

interface StoreContextValue {
  basket: Basket | null;
  setBasket: (basket: Basket) => void;
  // addItem は必要ない。addItem すると API は処理後の Basket を返すので Basket をセットすれば、
  // API 側と Basket の同期が取れるので addItem を context 側で実施して同期を取る必要がない
  removeItem: (productId: number, quantity: number) => void;
  // removeItem は Basket をレスポンスとして返さないので、
  // API 側と同期を取るために context 側でも実施する必要がある
}

export const StoreContext = createContext<StoreContextValue | undefined>(
  undefined
);

export function useStoreContext() {
  const context = useContext(StoreContext);

  if (context === undefined) {
    throw Error("Oops - we do not seem to be inside the provider");
  }

  return context;
}

export function StoreProvider({ children }: PropsWithChildren<any>) {
  const [basket, setBasket] = useState<Basket | null>(null);

  function removeItem(productId: number, quantity: number) {
    if (!basket) return;
    // state に直接変更をかけるよりもコピーを作成してから上書きする方がパフォーマンスがいい
    const items = [...basket.items];
    const itemIndex = items.findIndex((i) => i.productId === productId);
    // findIndex() は要素が見つからなければ -1 を返す
    if (itemIndex >= 0) {
      items[itemIndex].quantity -= quantity;
      if (items[itemIndex].quantity === 0) {
        // itemIndex から数えて  1 つだけ要素を削除した配列を返す。つまり、itemIndex の要素を削除する
        // basket.items のコピーに対して変更をかけている
        items.splice(itemIndex, 1);
      }
      setBasket((prevState) => {
        return { ...prevState!, items };
      });
    }
  }

  return (
    <StoreContext.Provider value={{basket, setBasket, removeItem}}>
        {children}
    </StoreContext.Provider>
  )
}
