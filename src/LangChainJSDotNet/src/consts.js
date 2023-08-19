export const unsupportedImportPrefixes = [
    "./agents/toolkits/",
    "./cache/",
    "./chat_models/",
    "./document_loaders/",
    "./document_transformers/",
    "./embeddings/",
    "./experimental/multimodal_embeddings/",
    "./llms/",
    "./memory/",
    "./retrievers/",
    "./storage/",
    "./stores/",
    "./tools/",  
    "./vectorstores/",   
]

export const supportedImports = [
    "./agents",
    "./agents/load",
    "./agents/toolkits",
    "./agents/toolkits/sql",
    "./base_language",
    "./tools",
    "./tools/calculator",
    "./tools/sql",
    "./chains",
    "./chains/load",
    "./chains/openai_functions",
    "./chains/query_constructor",
    "./chains/query_constructor/ir",
    "./chains/sql_db",
    "./embeddings/base",
    "./embeddings/fake",
    "./embeddings/openai",
    "./embeddings/cohere",
    "./llms/load",
    "./llms/base",
    "./llms/openai",
    "./llms/ai21",
    "./llms/aleph_alpha",
    "./llms/cohere",
    "./llms/hf",
    "./llms/ollama",
    "./llms/replicate",
    "./load/serializable",
    "./prompts",
    "./prompts/load",
    "./vectorstores/base",
    "./vectorstores/elasticsearch",
    "./vectorstores/memory",
    "./vectorstores/chroma",
    "./vectorstores/hnswlib",
    "./vectorstores/faiss",
    "./vectorstores/weaviate",
    "./vectorstores/lancedb",
    "./vectorstores/mongo",
    "./vectorstores/mongodb_atlas",
    "./vectorstores/pinecone",
    "./vectorstores/supabase",
    "./vectorstores/prisma",
    "./vectorstores/typesense",
    "./vectorstores/vectara",
    "./document",
    "./document_loaders/base",
    "./document_loaders/web/cheerio",
    "./document_loaders/web/puppeteer",
    "./document_loaders/web/playwright",
    "./document_loaders/web/college_confidential",
    "./document_loaders/web/gitbook",
    "./document_loaders/web/hn",
    "./document_loaders/web/imsdb",
    "./document_loaders/web/figma",
    "./document_loaders/web/notiondb",
    "./document_loaders/web/serpapi",
    "./document_loaders/web/sort_xyz_blockchain",
    "./document_loaders/fs/directory",
    "./document_loaders/fs/buffer",
    "./document_loaders/fs/text",
    "./document_loaders/fs/json",
    "./document_loaders/fs/srt",
    "./document_loaders/fs/pdf",
    "./document_loaders/fs/docx",
    "./document_loaders/fs/epub",
    "./document_loaders/fs/csv",
    "./document_loaders/fs/notion",
    "./document_loaders/fs/unstructured",
    "./document_transformers/html_to_text",
    "./document_transformers/openai_functions",
    "./chat_models/base",
    "./chat_models/openai",
    "./chat_models/baiduwenxin",
    "./chat_models/ollama",
    "./memory",
    "./schema",
    "./schema/output_parser",
    "./schema/query_constructor",
    "./schema/retriever",
    "./schema/runnable",
    "./sql_db",
    "./callbacks",
    "./output_parsers",
    "./output_parsers/expression",
    "./retrievers/remote",
    "./retrievers/supabase",
    "./retrievers/metal",
    "./retrievers/databerry",
    "./retrievers/contextual_compression",
    "./retrievers/document_compressors",
    "./retrievers/parent_document",
    "./retrievers/time_weighted",
    "./retrievers/document_compressors/chain_extract",
    "./retrievers/self_query",
    "./retrievers/self_query/chroma",
    "./retrievers/self_query/functional",
    "./retrievers/self_query/pinecone",
    "./retrievers/self_query/supabase",
    "./retrievers/self_query/weaviate",
    "./cache",
    "./storage/in_memory",
    "./stores/doc/in_memory",
    "./stores/file/in_memory",
    "./stores/message/in_memory",
    "./text_splitter",
    "./util/math",
    "./experimental/autogpt",
    "./experimental/babyagi",
    "./experimental/generative_agents",
    "./experimental/plan_and_execute",
    "./evaluation",
];