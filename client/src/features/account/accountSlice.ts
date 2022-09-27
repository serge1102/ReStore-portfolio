import { history } from "./../../index";
import { createAsyncThunk, createSlice, isAnyOf } from "@reduxjs/toolkit";
import { FieldValues } from "react-hook-form";
import agent from "../../app/api/agent";
import { User } from "../../app/models/user";
import { toast } from "react-toastify";
import { setBasket } from "../basket/basketSlice";

interface AccountState {
  user: User | null;
}

const initialState: AccountState = {
  user: null,
};

// <User, {data: FieldValues}> は User オブジェクトを返し、
// FieldValues 型が引数として必要ということ
export const signInUser = createAsyncThunk<User, FieldValues>(
  "account/signInUser",
  async (data, thunkAPI) => {
    try {
      const userDto = await agent.Account.login(data);
      const { basket, ...user } = userDto; // basket 以外の userDto の要素は user に集約される
      if (basket) thunkAPI.dispatch(setBasket(basket));
      // JSON.stringify() はオブジェクトや配列など、javascript の value を JSON 形式の文字列に変換する
      // ブラウザの Local Storage に token を保存
      localStorage.setItem("user", JSON.stringify(user));
      return user;
    } catch (error: any) {
      return thunkAPI.rejectWithValue({ error: error.data });
    }
  }
);

export const fetchCurrentUser = createAsyncThunk<User>(
  "account/currentUser",
  async (_, thunkAPI) => {
    // agent.Account.currentUser() は store に user がセットされている必要があるため、
    // 先に setUser() を実行する
    thunkAPI.dispatch(setUser(JSON.parse(localStorage.getItem("user")!))); // condition が true の場合だけ実行されるので必ず token はある
    try {
      const userDto = await agent.Account.currentUser();
      const { basket, ...user } = userDto; // basket 以外の userDto の要素は user に集約される
      if (basket) thunkAPI.dispatch(setBasket(basket));
      // JSON.stringify() はオブジェクトや配列など、javascript の value を JSON 形式の文字列に変換する
      localStorage.setItem("user", JSON.stringify(user));
      return user;
    } catch (error: any) {
      return thunkAPI.rejectWithValue({ error: error.data });
    }
  },
  {
    // Local Storage に token がなければ fetchCurrentUser は実行されない
    condition: () => {
      if (!localStorage.getItem("user")) return false;
    },
  }
);

export const accountSlice = createSlice({
  name: "account",
  initialState,
  reducers: {
    signOut: (state) => {
      state.user = null;
      localStorage.removeItem("user");
      history.push("/");
    },
    setUser: (state, action) => {
      state.user = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder.addCase(fetchCurrentUser.rejected, (state) => {
      state.user = null;
      localStorage.removeItem("user");
      toast.error("Session expired - please login again");
      history.push("/");
    });
    builder.addMatcher(
      isAnyOf(signInUser.fulfilled, fetchCurrentUser.fulfilled),
      (state, action) => {
        state.user = action.payload;
      }
    );
    builder.addMatcher(isAnyOf(signInUser.rejected), (state, action) => {
      throw action.payload;
    });
  },
});

export const { signOut, setUser } = accountSlice.actions;
