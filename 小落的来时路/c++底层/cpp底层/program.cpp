#include <iostream>
using namespace std;
// void foo(int *p){
//     if(p) *p = 7;
// }
//
// int main(){
//     int *ptr = nullptr;
//     foo(ptr);
//     cout << (ptr == nullptr) << endl;
// }

// class A{
// public:
//     A(){ std::cout << "A"; }
//     virtual ~A(){ std::cout << "~A"; }
// };
// class B: public A{
// public:
//     B(){ std::cout << "B"; }
//     ~B(){ std::cout << "~B"; }
// };
// int main(){
//     B* obj = new B();
//     delete obj;
//     return 0;
// }


// // 移动语义
// class A {
// public:
//     // 构造函数
//     A(int size) : size_(size) {
//         data_ = new int[size];
//     }
//     A(){}
//     // 拷贝构造
//     A(const A& a) {
//         size_ = a.size_;
//         data_ = new int[size_];
//         cout << "copy " << endl;
//     }
//     // 移动构造
//     A(A&& a) {
//         this->data_ = a.data_;
//         a.data_ = nullptr;  // 常数时间内完成，把原来指针置空
//         cout << "move " << endl;
//     }
//     // 析构
//     ~A() {
//         if (data_ != nullptr) {
//             delete[] data_;
//         }
//     }
//     int *data_;
//     int size_;
// };
// int main() {
//     A a(10);
//     A b = a;
//     A c = std::move(a); // 调用移动构造函数
//     return 0;
// }

// // 完美转发
// void PrintV(int &t) {
//     cout << "lvalue" << endl;
// }
//
// void PrintV(int &&t) {
//     cout << "rvalue" << endl;
// }
//
// template<typename T>
// void Test(T &&t) {
//     PrintV(t);						
//     PrintV(std::forward<T>(t));		
//     PrintV(std::move(t));			
// }
// // 以Test(1)为例
// // t作为形参 是左值
// // 1是右值 forward把t恢复成1 变成右值
// // move把t强行转换为右值
// // 以Test(std::forward<int&>(a))为例
// // t作为形参 是左值
// // a是int& 是左值 forward把t恢复成a 还是左值
// // move把t强行转换为右值
// int main() {
//     Test(1); // lvalue rvalue rvalue
//     int a = 1;
//     Test(a); // lvalue lvalue rvalue
//     Test(std::forward<int>(a)); // lvalue rvalue rvalue
//     Test(std::forward<int&>(a)); // lvalue lvalue rvalue
//     Test(std::forward<int&&>(a)); // lvalue rvalue rvalue
//     return 0;
// }



class Test {
public:
    Test() { std::cout << "Test::Test()\n"; }
    ~Test() { std::cout << "Test::~Test()\n"; }
    void test() { std::cout << "Test::test()\n"; }
};

int main() {
    std::shared_ptr<Test> sharedPtr(new Test());
    std::weak_ptr<Test> weakPtr = sharedPtr;
    
    if (auto lockedSharedPtr = weakPtr.lock()) {
        lockedSharedPtr->test();
    }
    // 当sharedPtr离开作用域时，它指向的对象会被自动销毁
    return 0;
}

