use std::time::Instant;

use rand::Rng;

fn main() {
    let max_size = 10000;
    let max_vertices = generate_rand_number(max_size);
    let mut graph: Vec<Vec<usize>> = Vec::new();
    for _ in 0..max_vertices {
        let mut row = Vec::new();
        for _ in 0..max_vertices {
            row.push(generate_rand_number(max_size));
        }
        graph.push(row);
    }

    println!("Graph Created");
    /// println!("{:?}", graph);

    let prim_graph = graph.clone();
    let start = Instant::now();
    let prim_mst = prim_mst(prim_graph, max_vertices);
    println!("Spend {:?} nanoseconds to run Prim Algorithm for {:?}", start.elapsed().as_nanos(), max_vertices);

    let kruskal_graph = convert_to_edges(graph.clone());
    let start = Instant::now();
    let kruskal_mst = kruskal_mst(kruskal_graph, max_vertices, false);
    println!("Spend {:?} nanoseconds to run Kruskal Algorithm for {:?}", start.elapsed().as_nanos(), max_vertices);
}

// utility functions
fn generate_rand_number(max_value: usize) -> usize {
    // add min value to avoid empty graph - 10..max_value
    return rand::thread_rng().gen_range(10..max_value);
}

// Prim's algorithm
// Reference: https://www.geeksforgeeks.org/prims-minimum-spanning-tree-mst-greedy-algo-5/
fn prim_mst(graph: Vec<Vec<usize>>, vert: usize) -> (Vec<usize>, Vec<usize>) {
    let mut parents: Vec<Option<usize>> = Vec::new();
    let mut keys: Vec<usize> = Vec::new();
    let mut mst_path: Vec<bool> = Vec::new();

    for i in 0..vert {
        mst_path.insert(i, false);
        keys.insert(i, usize::MAX);
        parents.insert(i, None);
    }

    keys.insert(0, 0);
    parents.insert(0, Some(0));

    for i in 0..vert-1 {
        let min_v = get_min_vertice(&keys, &mst_path, vert);
        mst_path[i] = true;

        for u in 0..vert {
            let current_vertice = graph.get(min_v).unwrap().get(u).unwrap();
            if *current_vertice != 0 && !mst_path.get(u).unwrap_or(&false) 
                && current_vertice < keys.get(u).unwrap() {
                parents[u] = Some(min_v);
                keys[u] = *current_vertice;
            }
        }
    }

    let mut result_vertices: Vec<usize> = Vec::new();
    let mut result_weights: Vec<usize> = Vec::new();

    for i in 1..parents.len() {
        let p = *parents.get(i).unwrap();
        if p.is_none() {
            continue;
        }

        result_vertices.push(p.unwrap());
        result_weights.push(*graph.get(i).unwrap().get(p.unwrap()).unwrap());
    }

    return (result_vertices, result_weights);
}

fn get_min_vertice(keys: &Vec<usize>, mst_path: &Vec<bool>, vert: usize) -> usize {
    let mut min = usize::MIN;
    let mut index: usize = 0;

    for i in 0..vert {
        if !mst_path.get(i).unwrap_or(&false) && keys.get(i).unwrap() < &min {
            min = *keys.get(i).unwrap();
            index = i;
        }
    }

    return index;
}

// Kruskal's algorithm
// Reference: https://www.geeksforgeeks.org/kruskals-minimum-spanning-tree-algorithm-greedy-algo-2/
fn kruskal_mst(graph: Vec<Vec<usize>>, vert: usize, convert_graph: bool) -> (Vec<usize>, Vec<usize>) {
    // sort nodes
    let mut edges: Vec<Vec<usize>>;
    if convert_graph {
        edges = convert_to_edges(graph);
    } else {
        edges = graph;
    }
    
    edges.sort_by(|a, b| a[2].cmp(&b[2]));
    let mut parents: Vec<usize> = Vec::new();
    let mut keys: Vec<usize> = Vec::new();

    for i in 0..vert {
        parents.insert(i, i);
        keys.insert(i, 0);
    }

    let mut result_vertices: Vec<usize> = Vec::new();
    let mut result_weights: Vec<usize> = Vec::new();

    for i in 0..vert {
        let e1 = edges[i][0];
        let e2 = edges[i][1];
        let v1 = find_parent(&mut parents, e1);
        let v2 = find_parent(&mut parents, e2);
        let weight = edges[i][2];

        if v1 != v2 {
            merge_path(v1, v2, &mut parents, &mut keys);
            result_weights.push(weight);
            result_vertices.push(v1);
        }
    }

    return (result_vertices, result_weights);
}

// Convert graph to edges like src, dest and weight
fn convert_to_edges(graph: Vec<Vec<usize>>) -> Vec<Vec<usize>> {
    let mut edges: Vec<Vec<usize>> = Vec::new();

    for i in 0..graph.len() {
        let row = graph.get(i).unwrap();
        for j in 0..row.len() {
            if j == i {
                continue;
            }

            let weight = *row.get(j).unwrap();
            if weight == 0 {
                continue;
            }

            let e: Vec<usize> = vec![i, j, weight];
            edges.push(e);
        }
    }
    return edges;
}

fn find_parent(parents: &mut Vec<usize>, vertice: usize) -> usize {
    if parents[vertice] == vertice {
        return vertice;
    }

    parents[vertice] = find_parent(parents, parents[vertice]);
    return parents[vertice];
}

fn merge_path(v1: usize, v2: usize, parents: &mut Vec<usize>, keys: &mut Vec<usize>) {
    let p1 = find_parent(parents, v1);
    let p2 = find_parent(parents, v2);

    if keys[p1] == keys[p2] {
        parents[p2] = p1;
        keys[p1] += 1;
    } else if keys[p1] > keys[p2] {
        parents[p2] = p1;
    } else {
        parents[p1] = p2;
    }
}