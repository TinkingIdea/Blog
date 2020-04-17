<template>
  <div>
    <!-- 面包屑导航区域 -->
    <el-breadcrumb separator-class="el-icon-arrow-right">
      <el-breadcrumb-item :to="{ path: '/home' }">首页</el-breadcrumb-item>
      <el-breadcrumb-item>文章管理</el-breadcrumb-item>
      <el-breadcrumb-item>文章列表</el-breadcrumb-item>
    </el-breadcrumb>

    <el-card>
      <el-row :gutter="20">
        <el-col :span="8">
          <el-input
            placeholder="请输入内容"
            v-model="queryInfo.query"
            class="input-with-select"
            clearable
          >
            <el-button slot="append" icon="el-icon-search"></el-button>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-select v-model="queryInfo.cate" placeholder="请选择分类" clearable >
            <el-option
              v-for="item in categoriesList"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            ></el-option>
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-button type="primary">添加文章</el-button>
        </el-col>
      </el-row>

      <el-table :data="articleList">
        <el-table-column label="#" type="index"></el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
export default {
  data() {
    return {
      queryInfo: {
        query: '',
        cate: '',
        pagenum: 1,
        pagesize: 10
      },
      articleList: [],
      categoriesList: [],
      total: 0
    }
  },
  created() {
    this.getArticleList()
    this.getCategoriesList()
  },
  methods: {
    getArticleList() {},
    async getCategoriesList() {
      const { data: res } = await this.$http.get('categories/list')
      if (res.meta.code !== 200) {
        return this.$message.error('获取分类列表失败')
      }
      this.categoriesList = res.data
    }
  }
}
</script>

<style lang="less" scoped></style>
